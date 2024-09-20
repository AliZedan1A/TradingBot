namespace trading_bot_3;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Reflection;
using System.Xml.Linq;
using trading_bot_3.Classes;
using Newtonsoft.Json;
using System.Diagnostics;
using System;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            App();
        }
        catch (Exception ex)
        {
            var error = ex.ToString();
            if (error.Contains("device_event_log_impl.cc(192)") && error.EndsWith("failed: Element not found. (0x490)"))
            {

            }
            else
            {
                Console.WriteLine(ex.ToString());
            }

            Task.Delay(-1).GetAwaiter().GetResult();
        }

    }
    public static void App()
    {
        if (!TradingView.Chose_Chart())
        {
            TradingView.Chose_Chart();
        }
        if(TradingView.My_Chart_Window.Mood == Mood.Binance)
        {
            if (!BinanceApi.Connect())
            {
                Console.WriteLine("Binance Error !!");
                return;
            }
            else
            {
                Console.WriteLine("Binance Connected");
            }
        } else if (TradingView.My_Chart_Window.Mood == Mood.BingX)
        {
            if (!BingXApi.Connect())
            {
                Console.WriteLine("BingX Error !!");
                return;
            }
            else
            {
                Console.WriteLine("BingX Connected");
            }
        }else if(TradingView.My_Chart_Window.Mood == Mood.MetaTredr)
        {
            new Thread(() =>
            {
                Fn.StartPython();
            }).Start();

        }

        else
        {
            Console.WriteLine("Test Mood");
        }


        Console.WriteLine($"{DateTime.UtcNow}: App Started - Coin ({TradingView.My_Chart_Window.ChartName.ProgramName})");
        Bot.Start();
        Algo.Run();
    }

}
