using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPal;
using PayPal.Api;
using SFM.Models;
using SFM.Models.PayPal;
using SFM.Models.ViewModels;

namespace SFM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayPalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PayPalController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private static string Escape(string str)
        {
            return str.Replace("Ä", "AE")
                .Replace("ä", "ae")
                .Replace("Ö", "OE")
                .Replace("ö", "oe")
                .Replace("Ü", "ue")
                .Replace("ü", "ue");
        }

        private async Task<APIContext> GetApiContext()
        {
            var isDebug = (await _context.Configs.FirstOrDefaultAsync(c => c.Key == Config.DEBUG)).Value == "1";
            PayPalConfig payPalConfig;
            if (isDebug)
            {
                var configs = new[] {Config.PAYPAL_TEST_CLIENT_ID, Config.PAYPAL_TEST_SECRET};
                var dbConfigs = await _context.Configs
                    .Where(c => configs.Contains(c.Key))
                    .ToListAsync();

                payPalConfig = new PayPalConfig
                {
                    ClientId = dbConfigs.FirstOrDefault(c => c.Key == Config.PAYPAL_TEST_CLIENT_ID)?.Value,
                    Secret = dbConfigs.FirstOrDefault(c => c.Key == Config.PAYPAL_TEST_SECRET)?.Value,
                    Mode = "sandbox"
                };
            }
            else
            {
                var configs = new[] {Config.PAYPAL_CLIENT_ID, Config.PAYPAL_SECRET};
                var dbConfigs = await _context.Configs
                    .Where(c => configs.Contains(c.Key))
                    .ToListAsync();

                payPalConfig = new PayPalConfig
                {
                    ClientId = dbConfigs.FirstOrDefault(c => c.Key == Config.PAYPAL_CLIENT_ID)?.Value,
                    Secret = dbConfigs.FirstOrDefault(c => c.Key == Config.PAYPAL_SECRET)?.Value,
                    Mode = "live"
                };
            }

            var config = new Dictionary<string, string>
            {
                {"clientId", payPalConfig.ClientId},
                {"clientSecret", payPalConfig.Secret},
                {"mode", payPalConfig.Mode},
                {"business", "codingsamuel-facilitator@gmail.com"}
            };

            var credential = new OAuthTokenCredential(config);
            var accessToken = credential.GetAccessToken();
            var context = new APIContext {Config = config, AccessToken = accessToken};
            return context;
        }

        private Plan CreatePlan(int interval, double price)
        {
            return new Plan
            {
                name = "Spotify Family",
                description = "Spotify Family Abonnement von Samuel Moutinho",
                type = "infinite",
                payment_definitions = new List<PaymentDefinition>
                {
                    new PaymentDefinition
                    {
                        name = "Spotify Family",
                        type = "regular",
                        frequency = "MONTH",
                        frequency_interval = interval.ToString(),
                        amount = new Currency
                        {
                            currency = "EUR",
                            value = price.ToString(CultureInfo.InvariantCulture)
                        },
                        cycles = "0"
                    }
                }
            };
        }

        private MerchantPreferences CreateMerchantPreferences(double price)
        {
            return new MerchantPreferences
            {
                setup_fee = new Currency
                {
                    currency = "EUR",
                    value = price.ToString(CultureInfo.InvariantCulture)
                },
                return_url = "https://localhost:5001/payment/success",
                cancel_url = "https://localhost:5001/payment/cancel",
                auto_bill_amount = "YES",
                initial_fail_amount_action = "CONTINUE",
                max_fail_attempts = "0"
            };
        }

        private ShippingAddress CreateShippingAddress(UserAddressViewModel address)
        {
            return new ShippingAddress
            {
                line1 = $"{Escape(address.Street)} {Escape(address.Number)}",
                city = Escape(address.City),
                state = Escape(address.State),
                postal_code = Escape(address.PostCode),
                country_code = "DE"
            };
        }

        private Agreement CreateAgreement(string planId, Address address)
        {
            return new Agreement
            {
                name = "Spotify Family Agreement",
                description = "Spotify Family Abonnement von Samuel Moutinho",
                start_date = DateTime.Now.AddMonths(1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                payer = new Payer
                {
                    payment_method = "paypal"
                },
                plan = new Plan {id = planId},
                shipping_address = address
            };
        }

        private PatchRequest CreatePatchRequest()
        {
            return new PatchRequest
            {
                new Patch
                {
                    op = "replace",
                    path = "/",
                    value = new PayPalModel
                    {
                        State = "ACTIVE"
                    }
                }
            };
        }

        private string GetLink(PayPalRelationalObject createdAgreement)
        {
            using var links = createdAgreement.links.GetEnumerator();
            while (links.MoveNext())
            {
                var link = links.Current;
                if (link != null && link.rel.ToLower().Trim().Equals("approval_url")) return link.href;
            }

            return null;
        }

        [HttpGet("[action]/{userId}")]
        public async Task<ActionResult<Subscription>> GetSubscription([FromRoute] long userId)
        {
            try
            {
                var dbSubscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.SpotifyUserId == userId);
                if (dbSubscription == null)
                    return NotFound();

                return dbSubscription;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("[action]/{userId}/{token}")]
        public async Task<ActionResult<Subscription>> ActivateSubscription([FromRoute] long userId,
            [FromRoute] string token)
        {
            try
            {
                var apiContext = await GetApiContext();

                // Update agreement
                var agreement = new Agreement {token = token};
                agreement.Execute(apiContext);

                // Get subscription
                var dbSubscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.SpotifyUserId == userId);
                if (dbSubscription == null)
                    return NotFound();

                // Update subscription
                dbSubscription.Token = token;
                dbSubscription.Active = true;
                dbSubscription.LastPayment = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(dbSubscription);
            }
            catch (ConnectionException ex)
            {
                return BadRequest(ex.Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{userId}/[action]")]
        public async Task<ActionResult<string>> Subscribe([FromRoute] long userId,
            [FromBody] SpotifySubscriptionViewModel model)
        {
            try
            {
                var apiContext = await GetApiContext();

                // Get config
                var priceConfig = await _context.Configs.FirstOrDefaultAsync(c => c.Key == Config.SPOTIFY_PRICE);
                var membersConfig = await _context.Configs.FirstOrDefaultAsync(c => c.Key == Config.SPOTIFY_MEMBERS);

                // Calculate price
                var totalPrice = double.Parse(priceConfig.Value);
                var members = int.Parse(membersConfig.Value);
                var price = totalPrice / members;

                // Create plan
                var plan = CreatePlan(model.Interval, price);
                var merchant = CreateMerchantPreferences(price);
                plan.merchant_preferences = merchant;
                var createdPlan = plan.Create(apiContext);

                var patchRequest = CreatePatchRequest();
                createdPlan.Update(apiContext, patchRequest);

                // Create agreement
                var shippingAddress = CreateShippingAddress(model.Address);
                var agreement = CreateAgreement(createdPlan.id, shippingAddress);
                var createdAgreement = agreement.Create(apiContext);

                // Get paypal link for checkout
                var link = GetLink(createdAgreement);

                // Add subscription
                await _context.Subscriptions.AddAsync(new Subscription
                {
                    SpotifyUserId = userId,
                    PaymentInterval = model.Interval,
                    Price = price,
                    Active = false
                });

                var address = _mapper.Map<UserAddress>(model.Address);
                address.SpotifyUserId = userId;
                await _context.UserAddresses.AddAsync(address);

                await _context.SaveChangesAsync();

                if (string.IsNullOrEmpty(link))
                    return BadRequest("Could not generate link...");

                return Ok(new PayPalSubscribeViewModel(link, createdAgreement.token));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}