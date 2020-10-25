 
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AppZeroAPI.Models
{
    public class PayPalApiClient
    {
        public ClientInfo GetClientSecrets(string path)
        {
            string file = System.IO.Path.Combine(path, "PayPal.json");
            string json = System.IO.File.ReadAllText(file);
            ClientInfo paypalSecrets = JsonConvert.DeserializeObject<ClientInfo>(json);
            return paypalSecrets;
        }

        public async Task<AccessToken> GetAccessToken(ClientInfo paypalSecrets)
        {
            // TODO: Store access token to reuse until expires
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en_US"));

                var byteArray = Encoding.ASCII.GetBytes(paypalSecrets.ClientId + ":" + paypalSecrets.ClientSecret);
                var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                client.DefaultRequestHeaders.Authorization = header;

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.sandbox.paypal.com/v1/oauth2/token");
                request.Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                });

                var response = await client.SendAsync(request);
                var result = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<AccessToken>(result);
            }
        }

        public string CreateOrder(ShoppingCart shoppingCart)
        {
            object data = new
            {
                intent = "order",
                payer = new
                {
                    payment_method = "paypal"
                },
                transactions = new List<object>
                {
                    new
                    {
                        amount = new
                        {
                            currency = "USD",
                            total = shoppingCart.GrandTotal,
                            details = new
                            {
                                shipping = shoppingCart.TotalShipping,
                                subtotal = shoppingCart.TotalExtendedPrice,
                                tax = "0.00"
                            }
                        },
                        payee = new
                        {
                            email = shoppingCart.payeeEmail
                        },
                        description = "Order from Detex, manufacturer of Deerfly Patches",
                        item_list = new
                        {
                            items = getPayPalItems(shoppingCart),
                            shipping_address = getPayPalAddress(shoppingCart.GetOrder().ShipTo)
                        }
                    }
                },
                redirect_urls = new
                {
                    return_url = "http://localhost:50138/Home/ShoppingCart",
                    cancel_url = "http://localhost:50138/Home/ShoppingCart"
                }
            };
            string dataJSON = JsonConvert.SerializeObject(data);
            return dataJSON;
        }

        public async Task<string> PostOrder(string data, string accessToken)
        {
            string result;
            string orderId = "";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.sandbox.paypal.com/v1/payments/payment");
                request.Content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await client.SendAsync(request);
                result = response.Content.ReadAsStringAsync().Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(result.ToString());
                }
                orderId = new JsonDeserializer(result).GetString("id");
            }
            return result;
        }

        private object getPayPalAddress(Address address)
        {
            return new
            {
                recipient_name = address.Recipient,
                line1 = address.Address1,
                line2 = address.Address2,
                city = address.City,
                country_code = address.Country,
                postal_code = address.Zip,
                phone = address.Phone,
                state = address.State
            };
        }

        private object getPayPalItem(OrderDetail orderDetail)
        {
            return new
            {
                name = orderDetail.Item.Name,
                quantity = orderDetail.Quantity,
                price = orderDetail.ExtendedPrice,
                sku = orderDetail.Item.ID,
                currency = "USD"
            };
        }

        private List<object> getPayPalItems(ShoppingCart shoppingCart)
        {
            List<object> items = new List<object>();
            List<OrderDetail> shoppingCartItems = shoppingCart.GetItems();

            for (int i = 0; i < shoppingCartItems.Count; i++)
            {
                items.Add(getPayPalItem(shoppingCartItems[i]));
            }
            return items;
        }
    }
}
