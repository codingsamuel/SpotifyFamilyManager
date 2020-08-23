using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PayPalController> _logger;
        private readonly IMapper _mapper;

        public PayPalController(ApplicationDbContext context, IMapper mapper, ILogger<PayPalController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _client = new HttpClient();
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

            _logger.LogInformation("IS DEBUG: " + JsonConvert.SerializeObject(isDebug));

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

            _logger.LogInformation("PAYPAL CONFIG: " + JsonConvert.SerializeObject(payPalConfig));

            var config = new Dictionary<string, string>
            {
                {"clientId", payPalConfig.ClientId},
                {"clientSecret", payPalConfig.Secret},
                {"mode", payPalConfig.Mode}
            };

            _logger.LogInformation("DICTIONARY CONFIG: " + JsonConvert.SerializeObject(config));

            // var credential = new OAuthTokenCredential(config);

            // _logger.LogInformation("CREDENTIAL: " + JsonConvert.SerializeObject(credential));

            // var accessToken = credential.GetAccessToken();

            var auth = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(payPalConfig.ClientId + ":" + payPalConfig.Secret));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"}
            });
            var request =
                await _client.PostAsync(
                    (payPalConfig.Mode == "live" ? "https://api.paypal.com" : "https://api.sandbox.paypal.com") +
                    "/v1/oauth2/token", content);
            var response = await request.Content.ReadAsStringAsync();

            _logger.LogInformation("RESPONSE: " + response);

            var token = JsonConvert.DeserializeObject<PayPalAccessTokenViewModel>(response);

            _logger.LogInformation("ACCESS TOKEN: " + token.Access_token);

            var context = new APIContext {Config = config, AccessToken = "Bearer " + token.Access_token};
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

        private async Task<MerchantPreferences> CreateMerchantPreferences(double price)
        {
            var config = await _context.Configs.FirstOrDefaultAsync(c => c.Key == Config.BASE_URL);

            return new MerchantPreferences
            {
                setup_fee = new Currency
                {
                    currency = "EUR",
                    value = price.ToString(CultureInfo.InvariantCulture)
                },
                return_url = $"{config.Value}payment/success",
                cancel_url = $"{config.Value}payment/cancel",
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

                _logger.LogInformation("GOT API CONTEXT....");

                // Get config
                var priceConfig = await _context.Configs.FirstOrDefaultAsync(c => c.Key == Config.SPOTIFY_PRICE);
                var membersConfig = await _context.Configs.FirstOrDefaultAsync(c => c.Key == Config.SPOTIFY_MEMBERS);

                _logger.LogInformation("GOT CONFIG....");

                // Calculate price
                var totalPrice = double.Parse(priceConfig.Value);
                var members = int.Parse(membersConfig.Value);
                var price = totalPrice / members;

                _logger.LogInformation("GOT PRICE....");

                // Create plan
                var plan = CreatePlan(model.Interval, price);
                var merchant = await CreateMerchantPreferences(price);
                plan.merchant_preferences = merchant;
                var createdPlan = plan.Create(apiContext);

                _logger.LogInformation("CREATED PLAN....");

                var patchRequest = CreatePatchRequest();
                createdPlan.Update(apiContext, patchRequest);

                _logger.LogInformation("ACTIVATED PLAN....");

                // Create agreement
                var shippingAddress = CreateShippingAddress(model.Address);
                var agreement = CreateAgreement(createdPlan.id, shippingAddress);
                var createdAgreement = agreement.Create(apiContext);

                _logger.LogInformation("CREATED AGREEMENT....");

                // Get paypal link for checkout
                var link = GetLink(createdAgreement);

                _logger.LogInformation("GOT LINK....");

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

                _logger.LogInformation("SAVED SUBSCRIPTION....");

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