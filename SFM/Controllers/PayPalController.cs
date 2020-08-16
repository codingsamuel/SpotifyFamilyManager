using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public PayPalController(ApplicationDbContext context)
        {
            _context = context;
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

        private Plan CreatePlan()
        {
            return new Plan
            {
                name = "Spotify Family",
                description = "Spotify Family Abonnement von Samuel Moutinho",
                type = "fixed",
                payment_definitions = new List<PaymentDefinition>
                {
                    new PaymentDefinition
                    {
                        name = "Spotify Family",
                        type = "regular",
                        frequency = "MONTH",
                        frequency_interval = "1",
                        amount = new Currency
                        {
                            currency = "EUR",
                            value = "2.50"
                        },
                        cycles = "1"
                    }
                }
            };
        }

        private MerchantPreferences CreateMerchantPreferences()
        {
            return new MerchantPreferences
            {
                setup_fee = new Currency
                {
                    currency = "EUR",
                    value = "2.50"
                },
                return_url = "https://localhost:5001/payment/success",
                cancel_url = "https://localhost:5001/payment/cancel",
                auto_bill_amount = "YES",
                initial_fail_amount_action = "CONTINUE",
                max_fail_attempts = "0"
            };
        }

        private ShippingAddress CreateShippingAddress()
        {
            return new ShippingAddress
            {
                line1 = "Schölerbergstr. 36",
                city = "Osnabrueck",
                state = "Lower Saxony",
                postal_code = "49082",
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

        [HttpPost("[action]/{userId}/{token}")]
        public async Task<ActionResult> ActivateSubscription([FromRoute] long userId, [FromRoute] string token)
        {
            try
            {
                var apiContext = await GetApiContext();

                var agreement = new Agreement {token = token};
                var executedAgreement = agreement.Execute(apiContext);

                var dbSubscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.SpotifyUserId == userId);

                if (dbSubscription == null)
                    return NotFound();

                dbSubscription.Active = true;
                dbSubscription.LastPayment = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(executedAgreement);
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
        public async Task<ActionResult<string>> Subscribe([FromRoute] long userId)
        {
            try
            {
                var apiContext = await GetApiContext();

                var plan = CreatePlan();
                var merchant = CreateMerchantPreferences();

                plan.merchant_preferences = merchant;
                var createdPlan = plan.Create(apiContext);
                var patchRequest = CreatePatchRequest();

                createdPlan.Update(apiContext, patchRequest);

                var shippingAddress = CreateShippingAddress();
                var agreement = CreateAgreement(createdPlan.id, shippingAddress);
                var createdAgreement = agreement.Create(apiContext);

                var link = GetLink(createdAgreement);

                await _context.Subscriptions.AddAsync(new Subscription
                {
                    SpotifyUserId = userId,
                    PaymentInterval = 30,
                    Price = 2.50,
                    Active = false
                });

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