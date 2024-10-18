using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PaymentGateway.Domain.Payment.TokenInfos.Dto;
using PaymentGateway.GatewayUi.Models;

namespace PaymentGateway.GatewayUi.Controllers
{
    public class GatewayController (IHttpClientFactory httpClientFactory, 
        IConfiguration config) : Controller
    {
        public async Task<IActionResult> Payment(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return View("NotFoundContent");
            }

            try
            {
                var baseUrl = config["Settings:PaymentBaseUrl"];
                var paymentSecretKey = config["Settings:PaymentSecretKey"];
                
                var client = httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("SecretKey", paymentSecretKey);

                var apiAddress = $"{baseUrl}/api/v1/Token/GetTokenInfo/{id}";
                var response = await client.GetAsync(apiAddress);
                
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return View("NotFoundContent");
                }
                
                var result = await response.Content.ReadFromJsonAsync<GetTokenInfoResponse>();
                if (result == null)
                {
                    return View("NotFoundContent");
                }

                var model = new PaymentViewModel
                {
                    Amount = result.Amount,
                    Cards = result.Cards?.Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = GetCardFormat(c.Pan, true)
                    }).ToList(),
                    CaptchaAddress = result.CaptchaAddress,
                    Token = id
                };

                var dis = result.ExpireDate - DateTime.Now;
                model.ExpireTime = dis.TotalMilliseconds;

                return View(model);
            }
            catch (Exception exp)
            {
                return View("Error");
            }
        }

        private readonly CardViewModel[] _cards =
        [
            new CardViewModel { Id = 1, CardNumber = "6037992518722564", Cvv2 = "570", ExpYear = "04", ExpMonth ="02" },
            new CardViewModel { Id = 2, CardNumber = "5411238536548754", Cvv2 = "780", ExpYear = "05", ExpMonth ="05" },
            new CardViewModel { Id = 3, CardNumber = "6037845887647863", Cvv2 = "3125", ExpYear = "06", ExpMonth ="07" },
            new CardViewModel { Id = 4, CardNumber = "5487963214569854", Cvv2 = "254", ExpYear = "08", ExpMonth ="11" }
        ];

        private string? GetCardFormat(string? cardNumber, bool isMusked = false)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return null;

            if (isMusked && cardNumber.Length == 16)
            {
                cardNumber = cardNumber.Substring(0, 6) + "******" + cardNumber.Substring(12, 4);
            }

            var retStr = string.Empty;

            for (var i = 1; i <= cardNumber.Length; i++)
            {
                retStr += i % 4 == 0 ? cardNumber[i - 1] + " " : cardNumber[i - 1];
            }

            return retStr;
        }

        [HttpPost]
        public IActionResult SendCardData(PaymentViewModel data)
        {
            try
            {
                return PartialView();
            }
            catch (Exception exp)
            {
                return Json(null);
            }
        }

        [HttpPost]
        public IActionResult CardListChange(string selectedValue)
        {
            if (!long.TryParse(selectedValue, out var cardId))
            {
                return Json(null);
            }

            var card = _cards.FirstOrDefault(c => c.Id == cardId);
            if (card == null)
            {
                return Json(null);
            }

            var response = new CardViewModel
            {
                Id = card.Id,
                CardNumber = GetCardFormat(card.CardNumber),
                Cvv2 = card.Cvv2,
                ExpMonth = card.ExpMonth,
                ExpYear = card.ExpYear,
            };

            return Json(response);
        }
    }
}
