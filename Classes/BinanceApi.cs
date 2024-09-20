using Binance.Spot;
using Binance.Spot.Models;
using Binance.Common;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace trading_bot_3;

public class Chart
{
    
    public string Name { get; set; }
    public string ProgramName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PathKey { get; set; }
    public int Min { get; set; }
    public decimal MaxPersent { get; set; }
    public decimal Money { get; set; }
}
public class Binance_Coin
{
    public string coin { get; set; }
    public string free { get; set; }

}

public class BinanceApi {
    private static string GetApiKey()
    {
        return TradingView.GetConfigJson().Binance_ApiKey;
    }
    private static string GetSecret()
    {
        return TradingView.GetConfigJson().BingX_ApiSec;
    }
    public static string BaseUrl = "https://api.binance.com";
    public static  bool Connect(){
        try
        {
            var httpClient = new HttpClient();
            var wallet = new Wallet(httpClient, apiKey: GetApiKey(), apiSecret: GetSecret(), baseUrl: BaseUrl);
            var result = Fn.Await(wallet.AccountStatus());
            return true;
        } catch (Exception ex)
        {
            return false;
        }

    }
    public static async Task<bool> Buy(Chart sym, decimal qty)
    {
        try{
            var httpClient = new HttpClient();
            var spotAccountTrade = new SpotAccountTrade(httpClient, apiKey: GetApiKey(), apiSecret: GetSecret(), baseUrl: BaseUrl);
            var result = await spotAccountTrade.NewOrder(sym.ProgramName.ToString().Replace("_", ""), Side.BUY, OrderType.MARKET, quoteOrderQty: qty);

            return true;
        } catch(BinanceClientException ex){
            Fn.SendWebHocErrorkMessage($"***{DateTime.UtcNow}***\n***({sym.ProgramName}) Buy Error:*** " +  ex.Message + $"***USDT qyt:*** \n{qty}");
            Fn.UTCTimeLog(ex.Message);
            return false;
        }

    }
    public static async Task<bool> Sell (Chart sym)
    {
        var coin = Get_Coin_Wallet_Value(sym.ProgramName.Split("_")[0]);
        try
        {
            var httpClient = new HttpClient();
            var spotAccountTrade = new SpotAccountTrade(httpClient, apiKey: GetApiKey(), apiSecret: GetSecret(), baseUrl: BaseUrl);
            var result = await spotAccountTrade.NewOrder(sym.ProgramName.Replace("_", ""), Side.SELL, OrderType.MARKET, quantity: Fn.Get_Untel_Two(decimal.Parse(coin.free)));

            return true;
        }
        catch (BinanceClientException ex)
        {
            Fn.SendWebHocErrorkMessage($"***{DateTime.UtcNow}***\n***({sym.ProgramName}) Sell Error:*** " + ex.Message + $"***{coin.coin} qyt:*** \n{Fn.Get_Untel_Two(decimal.Parse(coin.free))}");
            Fn.UTCTimeLog(ex.Message);
            return false;
        }
    }
    public static Binance_Coin Get_Coin_Wallet_Value(string coin)
    {
        try
        {
            var httpClient = new HttpClient();
            var wallet = new Wallet(httpClient, apiKey: GetApiKey(), apiSecret: GetSecret(), baseUrl: BaseUrl);
            var result = Fn.Await(wallet.AllCoinsInformation());
            var jsonRsolt = JsonConvert.DeserializeObject<Binance_Coin[]>(result)!;
            foreach(var c in jsonRsolt)
            {
                if(c.coin == coin.ToUpper())
                {
                    return c;
                }
            }
        } catch(BinanceClientException ex)
        {
            Console.WriteLine(ex.Message);
            return new();
        }
        return new();

    }


}