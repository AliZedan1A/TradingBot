using Binance.Spot.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace trading_bot_3.Classes
{
    public class Bingx_Coin
    {
        public string asset { get; set; }
        public decimal free { get; set; }

    }

    internal class BingXApi
    {
        private static string BaseUrl = "https://open-api.bingx.com/openApi";
        public static Bingx_Coin Get_Coin_Wallet_Value(string coin)
        {
            var res =  Req("/spot/v1/account/balance", HttpMethod.Get, new
            {
                
            }).GetAwaiter().GetResult();
            JToken data = JObject.Parse(res)["data"]!["balances"]!;
            foreach (var c in data)
            {
                if ((string)c["asset"]! == coin)
                {
                    return new() { 
                        asset = (string)c["asset"]!,
                        free = (decimal)c["free"]!,
                    };
                }
            }
            return new();
        }
        public static bool Connect()
        {
            try
            {
                Get_Coin_Wallet_Value("USDT");
                return true;
            } catch(Exception ex)
            {
                return false;
            }
        }


        public static async Task<bool> Buy(Chart sym, decimal qty)
        {
            Get_Coin_Wallet_Value("USDT");
            try
            {
                var res = await Req("/spot/v1/trade/order", HttpMethod.Post, new
                {
                    symbol = sym.ProgramName.Replace("_", "-"),
                    side = "BUY",
                    type = "MARKET",
                    quoteOrderQty = qty,
                });
                Console.WriteLine(res);
                return true;
            } catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

        }
        public static async Task<bool> Sell(Chart sym)
        {
            var coin = sym.ProgramName.Split("_")[0];
            var coinQty = Get_Coin_Wallet_Value(coin);
            try
            {
                var res = await Req("/spot/v1/trade/order", HttpMethod.Post, new
                {
                    symbol = sym.ProgramName.Replace("_", "-"),
                    side = "SELL",
                    type = "MARKET",
                    quantity = coinQty.free,
                });
                return true;
            } catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }


        }


        public static async Task<string> Req<T>(string path, HttpMethod method, T payload)
        {
            string API_KEY = TradingView.GetConfigJson().BingX_ApiKey;
            string API_SECRET = TradingView.GetConfigJson().BingX_ApiSec;
            long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            string parameters = $"timestamp={timestamp}";

            if (payload != null)
            {
                foreach (var property in payload.GetType().GetProperties())
                {
                    parameters += $"&{property.Name}={property.GetValue(payload)}";
                }
            }

            string sign = CalculateHmacSha256(parameters, API_SECRET);
            string url = $"{BaseUrl}{path}?{parameters}&signature={sign}";

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    HttpRequestMessage msg = new(method, url);
                    msg.Headers.Add("X-BX-APIKEY", API_KEY);
                    var response = await client.SendAsync(msg);
                    string responseBody = await response.Content.ReadAsStringAsync();
                    int error_code = (int)JObject.Parse(responseBody)!["code"]!;
                    if (!response.IsSuccessStatusCode || error_code != 0)
                    {
                        Console.WriteLine("BingX Error !!");
                        throw new Exception(responseBody);
                    }
                    
                    return responseBody;
                }
            }
        }

        public static string CalculateHmacSha256(string input, string key)
        {
            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            using (HMACSHA256 hmac = new HMACSHA256(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

    }
}
