using Newtonsoft.Json;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using trading_bot_3;
using trading_bot_3.Classes;

namespace trading_bot_3;


public enum Mood
{
    Test = 1,
    Binance = 2,
    BingX = 3,
    MetaTredr = 4
}
public enum Time_Choice
{
    By_1_Min = 1,
    By_3_Min = 2,
    By_5_Min = 3,
    //By_15_Min = 4,
    //By_30_Min = 5,
    //By_45_Min = 6,
    //By_1_hour = 7,
    //By_2_hour = 8,
    //By_3_hour = 9,
    //By_4_hour = 10,
}
public class Point_colors
{
    public static readonly string Green = "rgb(0, 230, 118)";
    public static readonly string Red = "rgb(255, 82, 82)";
}
public class Time_Value
{
    public DateTime Time { get; set; }
    public decimal Price { get; set; }
}
public class But_Sell_Swings
{
    public static Time_Value? Last_Sell = null;
    public static Time_Value? Last_Buy = null;
}
public class Plot_Obj
{
    public float RSI { get; set; }
}
public enum CandleType
{
    Top,
    Bottom,
}
public class Candle
{
    public Price_Values CandleProprtey { get; set; }
    public float RsiValue { get; set; }
    public DateTime UTCtime { get; set; } = DateTime.UtcNow;
    public int Id { get; set; } = Trading_View_Chart_Window.Candles.Count ==0 ? 1 : Trading_View_Chart_Window.Candles.Count+1;


}
public class CandleTB : Candle
{
    public CandleType Candletype { get; set; }
}
public class Price_Values
{
    public decimal Open { get; set; }
    public decimal Close { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
}
public class Plot_Key_Value<T>
{
    public string name { get; set; }
    public T value { get; set; }
}
public class JSONCONFIG
{
    public string WebHock { get; set; }
    public string UpdatesWebhock { get; set; }
    public string ErrorsWebhock { get; set; }
    public List<Chart> Coins { get; set; }
    public string Binance_ApiKey { get; set; }
    public string Binance_ApiSec { get; set; }
    public string BingX_ApiSec { get; set; }
    public string BingX_ApiKey { get; set; }
    public string MetaAcc { get; set; }
    public string MetaPass { get; set; }
    public string MetaServer { get; set; }
    public double MetaLot { get; set; }

}
public class TradingView
{
    private static readonly string BaseUrl = "https://www.tradingview.com";
    public static Trading_View_Chart_Window My_Chart_Window = new();
    private static JSONCONFIG SavedJsonConfig = null;
    public static JSONCONFIG GetConfigJson()
    {
        if (SavedJsonConfig == null)
        {
            var file_data = File.ReadAllText($"{Fn.Root_Path()}/Data/config.json");
            SavedJsonConfig = JsonConvert.DeserializeObject<JSONCONFIG>(file_data)!;
            return SavedJsonConfig;
        }
        else
        {
            return SavedJsonConfig;
        }

    }
    private static void GoTo(string path)
    {
        Bot.SetUrl($"{BaseUrl}{path}");
    }
    public static bool Chose_Chart()
    {
        var charts_list = GetConfigJson().Coins;
        var i = 1;
        foreach (var ch in charts_list)
        {
            Console.WriteLine($"{i} - {ch.ProgramName}");
            i++;
        }
        Console.Write("Choice: ");
        var choice = Console.ReadLine()!;
        int c_index = 0;
        if (!int.TryParse(choice, out c_index))
        {
            Console.WriteLine("Invalid Input");
            return false;
        }
        if (c_index > charts_list.Count)
        {
            Console.WriteLine("Invalid Input");
            return false;
        }
        var chart = charts_list[c_index - 1];

        var moods = Enum.GetValues(typeof(Mood));
        var j = 1;
        foreach (var md in moods)
        {
            Console.WriteLine($"{j} - {md}");
            j++;
        }
        Console.Write("Choice: ");
        var choice_mood = Console.ReadLine()!;
        int c_nood_index = 0;
        if (!int.TryParse(choice_mood, out c_nood_index))
        {
            Console.WriteLine("Invalid Input");
            return false;
        }
        if (c_index > charts_list.Count)
        {
            Console.WriteLine("Invalid Input");
            return false;
        }
        My_Chart_Window.Mood = (Mood)c_nood_index;

        My_Chart_Window.ChartName = chart;
        My_Chart_Window.Money = chart.Money;
        My_Chart_Window.Max_Persent = chart.MaxPersent;
        My_Chart_Window.time_Choice = chart.Min == 3 ? Time_Choice.By_3_Min : chart.Min == 5 ? Time_Choice.By_5_Min : Time_Choice.By_1_Min;
        Console.WriteLine($"your mood is: {My_Chart_Window.Mood}");
        Console.WriteLine($"your chart is: {My_Chart_Window.ChartName.ProgramName}");
        Console.WriteLine($"your max percent is: {My_Chart_Window.Max_Persent}%");
        Console.WriteLine($"your money is: {My_Chart_Window.Money}$");
        Console.WriteLine($"your time is: {My_Chart_Window.time_Choice.ToString()}");
        Console.WriteLine($"your email is: {My_Chart_Window.ChartName.Email}");
        return true;
    }
    public static Price_Values Now_Price()
    {
        var values_prices_text = Bot.QuerySelectorAll(".values-_gbYDtbd")[1].Text;
        var close = values_prices_text.Split("Close")[1].Split("Change")[0];
        var open = values_prices_text.Split("Open")[1].Split("High")[0];
        var high = values_prices_text.Split("High")[1].Split("Low")[0];
        var low = values_prices_text.Split("Low")[1].Split("Close")[0];
        return new()
        {
            Close = decimal.Parse(close),
            Open = decimal.Parse(open),
            High = decimal.Parse(high),
            Low = decimal.Parse(low),
        };
    }
    public static void Login()
    {
        GoTo("/");
        // open user menu
        Bot.QuerySelector(".js-header-user-menu-button").Click();
        Fn.Sleep(1);
        // click singin button
        Bot.QuerySelectorAll(".item-mDJVFqQ3")[1].Click();
        Fn.Sleep(1);
        // click singin by email button
        Bot.QuerySelector("button[name=Email]").Click();
        Fn.Sleep(1);
        // send email data to (username or email)input
        Bot.QuerySelector("#id_username").SendKeys(My_Chart_Window.ChartName.Email);
        // send password data to password input
        Bot.QuerySelector("#id_password").SendKeys(My_Chart_Window.ChartName.Password);
        // click logon button
        Bot.QuerySelector(".submitButton-LQwxK8Bm").Click();
        Console.Write("Press Enter...");
        Console.ReadLine();
    }

}

public class Trading_View_Chart_Window
{
    public Chart ChartName { get; set; }
    public decimal Money { get; set; }
    public decimal Max_Persent { get; set; }
    public static decimal DaileyPersent = 0;
    public static int DaileyTrades = 0;
    public Mood Mood = Mood.Test;
    public Time_Choice time_Choice { get; set; }
    public static List<Candle>Candles = new List<Candle>();
    public static List<CandleTB> TopsBottomsCandles = new List<CandleTB>();
    public static List<CandleTB> DiversCandels = new List<CandleTB>();

    private static Candle GetNowCandel(Plot_Obj plot)
    {
        return new()
        {
            CandleProprtey = TradingView.Now_Price(),
            RsiValue = plot.RSI,
            UTCtime = DateTime.UtcNow
        };
    }
    private static readonly string BaseUrl = "https://www.tradingview.com";
    public void OpenChart()
    {
        Bot.SetUrl($"{BaseUrl}/chart/{ChartName.PathKey}/?symbol={ChartName.Name}");
        Fn.Sleep(4);
    }
    public void Close_Ad()
    {
        try
        {
            Fn.Sleep(2);
            Bot.QuerySelector(".close-button-FuMQAaGA").Click();
            Fn.Sleep(2);
        }
        catch (Exception ex)
        {

        }
    }
    public void Open_Buy_Sell_Indicator()
    {
        // code error
        var Indicators_Button = Bot.QuerySelector("#header-toolbar-indicators button");
        Indicators_Button.Click();
        Fn.Sleep(2);
        Close_Ad();
        var Ilist = Bot.QuerySelectorAll(".container-WeNdU0sq");
        Console.WriteLine(Ilist.Count);
        Ilist[0].Click();
        Fn.Sleep(2);
        Ilist[1].Click();
        Close_Ad();
        var close_Indicator_Button = Bot.QuerySelector(".close-BZKENkhT");
        close_Indicator_Button.Click();
        Close_Ad();
    }
    public void RemoveAllIndectors()
    {
        var nav = Bot.QuerySelectorAll(".button-KTgbfaP5");
        nav.Last().Click();
        var options = Bot.QuerySelectorAll(".accessible-NQERJsv9");
        options.Last().Click();
    }
    public void Chose_Time()
    {
        var time_menu = Bot.QuerySelectorAll(".menu-S_1OCXUK")[2];
        time_menu.Click();
        Fn.Sleep(2);
        Close_Ad();
        var all_time_menu_list = Bot.QuerySelectorAll(".labelRow-jFqVJoPk");
        var times_list = Enum.GetNames(typeof(Time_Choice)).ToList();
        var index = 5;
        for (int i = 0; i < times_list.Count; i++)
        {
            if (time_Choice.ToString() == times_list[i])
            {
                all_time_menu_list[index].Click();
                Fn.Sleep(2);
                Close_Ad();
                break;
            }
            index++;
        }
    }


    private Plot_Key_Value<float> Get_Plot_Key_Value(IList<IWebElement> fether_box, int index_box, int index_name, int index_value)
    {
        var box = fether_box[fether_box.Count - index_box];
        var values_box_List = box.Text.Split("\n");
        var name = values_box_List[values_box_List.Length - index_name];
        var value_str = values_box_List[values_box_List.Length - index_value];
        float value = 0;
        if (float.TryParse(value_str, out value) == false)
        {
            string num = string.Empty;
            foreach (var item in value_str)
            {

                if (float.TryParse(item.ToString(), out value) || item == '.')
                {
                    num = num + item;
                }
            }
            float.TryParse(num, out value);
            value = 0 - value;
        }
        return new()
        {
            name = name.Remove(name.Length - 1),
            value = value,
        };
    }
    private float Get_Plot_Value(IList<IWebElement> fether_box, int index_box, int index_value)
    {
        var box = fether_box[fether_box.Count - index_box];
        var values_box_List = box.Text.Split("\n");
        var value_str = values_box_List[values_box_List.Length - index_value];
        float value = 0;
        if (float.TryParse(value_str, out value) == false)
        {
            string num = string.Empty;
            foreach (var item in value_str)
            {

                if (float.TryParse(item.ToString(), out value) || item == '.')
                {
                    num = num + item;
                }
            }
            float.TryParse(num, out value);
            value = 0 - value;
        }
        return value;
    }
    private Plot_Key_Value<string> Get_Plot_Key_Value_Str(IList<IWebElement> fether_box, int index_box, int index_name, int index_value)
    {
        var box = fether_box[fether_box.Count - index_box];
        var values_box_List = box.Text.Split("\n");
        var name = values_box_List[values_box_List.Length - index_name];
        var value = values_box_List[values_box_List.Length - index_value];
        return new()
        {
            name = name.Remove(name.Length - 1),
            value = value
        };
    }

    public Plot_Obj Get_Plot()
    {
        var value_boxes = Bot.QuerySelectorAll(".values-_gbYDtbd");
        // box = 1
        var rsi = Get_Plot_Value(value_boxes, 1, 17);

        return new()
        {
            RSI = rsi,
        };
    }

    public void Click_Data_Windwo()
    {
        var data_window = Bot.QuerySelectorAll(".light-tab-button-_v4jz9BC")[1];

        data_window.Click();
        Fn.Sleep(2);
    }
    public bool Is_Plot_Elemnt_found()
    {
        return true;
    }

   bool AreWeInOrder = false;
    public void Algo()
    {
       // var data_window = Bot.QuerySelectorAll(".light-tab-button-_v4jz9BC")[1];
        //data_window.Click();
        Fn.Sleep(2);
        Console.Write("Am Ready Boss\nWaiting For You To Take The Action...");
        Console.ReadLine();

        new Thread(() =>
        {
            while (true)
            {
                
                Thread.Sleep(100);
                var value_boxes = Bot.QuerySelectorAll(".values-_gbYDtbd");
                var array = value_boxes.Last().Text.Split("\n");// 1 = buy  , 3=sell
                var sell =  array[3];
                var buy = array[1];
                double tpD;
                double slD;
                var tp = double.TryParse(array[5] ,out tpD);
                var sl = double.TryParse(array[7],out slD);
                if(double.Parse(sell) > 0.0)
                {
                   

                    Order(1, tpD, slD);
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                }
                if (double.Parse(buy) > 0.0)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    Order(0, tpD, slD);
                    stopwatch.Stop();
                    Console.WriteLine($"Elapsed Time: {stopwatch.ElapsedMilliseconds} ms");
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                }

            }
        }).Start();
    }

    private bool Order(int Ord ,double tp, double sl)
    {
        bool is_Buy = Mood == Mood.Binance ? BinanceApi.Buy(ChartName, Money).GetAwaiter().GetResult()
            : Mood == Mood.BingX ? BingXApi.Buy(ChartName, Money).GetAwaiter().GetResult()
            : Mood == Mood.Test;
        var lotsize = TradingView.GetConfigJson().MetaLot;

        string buyorsell = Ord == 0 ? "BUY"
            : Ord == 1 ? "Sell" : "sell";
        MetaTreder mt = new MetaTreder();
        string qurey = $"1,{buyorsell},{lotsize},{tp},{sl}";
        var x= mt.Command(qurey);
        Console.WriteLine($"{x.Sus}  {x.Tp} {x.PriceOpen} {x.Comment}");
        if (x.Sus)
        {
            Fn.SendWebHockMessage($"***XAUUSD صفقة جديدة***\n **Entery :{x.PriceOpen}**  \n **TP : {x.Tp}** \n **SL : {x.Sl}\n **Order Ticket {x.OrderTicket}** \n **Comment : {x.Comment}** ");
            return true;
        }
        else
        {
            Fn.SendWebHockMessage($"***فشل فتح صفقة *** \n Comment : {x.Comment}");
            return false;
        }

    }
    private bool CloseTime()
    {

#if DEBUG
        return true;
#else
        var datenow = DateTime.UtcNow;
        var suc = int.Parse(datenow.ToString("ss"));
        var min = int.Parse(datenow.ToString("mm"));

        switch (time_Choice)
        {
            case Time_Choice.By_1_Min:
                if (suc >= 59)
                {
                    return true;
                }
                break;
            case Time_Choice.By_5_Min:
                if (((min + 1) % 5) == 0 && suc >= 59)
                {
                    return true;
                }
                break;
            case Time_Choice.By_3_Min:
                if (((min + 1) % 3) == 0 && suc >= 59)
                {
                    return true;
                }
                break;
        }
        return false;
#endif
    }

}
