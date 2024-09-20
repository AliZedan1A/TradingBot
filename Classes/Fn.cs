using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;

namespace trading_bot_3;

public class Fn {

    // candes
    private static Candle GetOldCandle()
    {
        //DateTime candledate =DateTime.UtcNow;
        //Candle c = new Candle();
        //foreach(var item in Trading_View_Chart_Window.Candles)
        //{
        //    if(item.UTCtime< candledate)
        //    {
        //        c = item;
        //    }

        //}
        //return c;
        return Trading_View_Chart_Window.Candles.Find(x=>x.Id==1);
    }
    private static CandleTB GetOldCandleDates(CandleTB a , CandleTB b)
    {
        return a.UTCtime < b.UTCtime ? b : a;
    }
  
    public static void GetDrivce()
    {
        int i = 0;
        int j = 0;
        foreach (var top_or_bottom in Trading_View_Chart_Window.TopsBottomsCandles)
        {

            foreach (var top_or_bottom_2 in Trading_View_Chart_Window.TopsBottomsCandles)
            {
                void add()
                {
                    var ca = GetOldCandleDates(top_or_bottom, top_or_bottom_2);
                    //if (Trading_View_Chart_Window.DiversCandels.SingleOrDefault(x => x.UTCtime == ca.UTCtime) is null)
                    if (Trading_View_Chart_Window.DiversCandels.SingleOrDefault(x => x.Id == ca.Id) is null)
                    {
                        Trading_View_Chart_Window.DiversCandels.Add(ca);
                        
                    }
                }
                if(i == j)
                {
                    continue;
                }
                if (top_or_bottom.Candletype == CandleType.Bottom && top_or_bottom_2.Candletype == CandleType.Bottom)
                {
                    if (top_or_bottom.CandleProprtey.Low > top_or_bottom_2.CandleProprtey.Low && top_or_bottom.RsiValue < top_or_bottom_2.RsiValue)
                    {
                        add();
                    }
                }
                if (top_or_bottom.Candletype == CandleType.Top && top_or_bottom_2.Candletype == CandleType.Top)
                {
                    if (top_or_bottom.CandleProprtey.High > top_or_bottom_2.CandleProprtey.High && top_or_bottom.RsiValue < top_or_bottom_2.RsiValue)
                    {
                        add();
                    }
                }
                j++;
            }
            i++;
        }
    }



    public static void TopsBottomsC(int bwtess)
    {
    l:;
        if (Trading_View_Chart_Window.Candles.Count != 20 || (bwtess % 2) == 0 ) return;
        bool istb = true;
        int i = 0;
        List<Candle> alog_candel = new();
        for (int j = 0 ; j < bwtess; j++)
        {
            alog_candel.Add(Trading_View_Chart_Window.Candles[j]);
        }
        var c_mid = alog_candel[((alog_candel.Count - 1) / 2) + 1];
        //if(Trading_View_Chart_Window.TopsBottomsCandles.SingleOrDefault(x=>x.UTCtime == c_mid.UTCtime) is  null)
        if (Trading_View_Chart_Window.TopsBottomsCandles.SingleOrDefault(x => x.Id == c_mid.Id) is null)
        {
            if(c_mid.RsiValue>45)
            {
                foreach(var item in alog_candel)
                {
                    if(c_mid.CandleProprtey.High<item.CandleProprtey.High)
                    {
                        istb = false;
                    }
                }
                if(istb)
                {
                    Trading_View_Chart_Window.TopsBottomsCandles.Add(new CandleTB() {
                        CandleProprtey = c_mid.CandleProprtey,
                        RsiValue = c_mid.RsiValue,
                        UTCtime = c_mid.UTCtime,
                        Id = c_mid.Id,
                        Candletype =CandleType.Top 
                    });
                }
            }
            else
            {
                foreach (var item in alog_candel)
                {
                    if (c_mid.CandleProprtey.Low < item.CandleProprtey.Low)
                    {
                        istb = false;
                    }
                }
                if (istb)
                {
                    Trading_View_Chart_Window.TopsBottomsCandles.Add(new CandleTB() {
                        CandleProprtey = c_mid.CandleProprtey,
                        RsiValue = c_mid.RsiValue,
                        UTCtime = c_mid.UTCtime,
                        Id = c_mid.Id,
                        Candletype = CandleType.Bottom 
                    });
                }
            }
        }
        i++;
        if (i > 16) return;
        goto l;



    }
    private static int c_id = 0;
    public static void AddCandle(Candle candle)
    {
        foreach(var item in Trading_View_Chart_Window.Candles)
        {
            if (item.UTCtime.Second == candle.UTCtime.Second) return;
        }
        if(Trading_View_Chart_Window.Candles.Count>=20)
        {
            Trading_View_Chart_Window.Candles.Remove(GetOldCandle());

        }
        Trading_View_Chart_Window.Candles.Add(candle);
        Console.WriteLine(candle.RsiValue);
        TopsBottomsC(5);

    }
    // candels

    public static void Sleep(int suc)
    {
        Await(
            Task.Delay(TimeSpan.FromSeconds(suc))
        );
    }
    public static decimal GetPercentOfTwo(decimal big, decimal small)
    {
        if (big > small)
        {
            var final = ((decimal)(((big - small) / big) * 100)).ToString("0.00");
            return decimal.Parse(final);
        }
        else
        {
            var final = ((decimal)(((small - big) / small) * 100)).ToString("-0.00");
            return decimal.Parse(final);

        }
    }
    public static string Root_Path()
    {
        return System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
    }
    public static T Await<T>(Task<T> task){
        return task.GetAwaiter().GetResult();
    }
     public static void Await(Task task){
        task.GetAwaiter().GetResult();
    }
    public static async void StartPython()
    {
        string command = "python Data/python/Main.py"; 
        ProcessStartInfo processInfo = new ProcessStartInfo();
        processInfo.FileName = "cmd.exe"; 
        processInfo.Arguments = "/c " + command; 
        processInfo.RedirectStandardOutput = true; 
        processInfo.RedirectStandardError = true; 
        processInfo.UseShellExecute = false;

        Process process = new Process();
        process.StartInfo = processInfo;

        try
        {
            process.Start(); 

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            Console.WriteLine("Output:");
            Console.WriteLine(output);

            process.WaitForExit(); 

            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("Error:");
                Console.WriteLine(error);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception:");
            Console.WriteLine(ex.Message);
        }
    }
    public static async void SendWebHockMessage(string text)
    {
        new Thread(() =>
        {
            try
            {
                var config = TradingView.GetConfigJson();
                WebClient msgstart = new WebClient();
                msgstart.UploadValues(config.WebHock, new NameValueCollection
                {
                    {
                        "content",
                        text ?? ""
                    },
                    {
                        "username",
                        "FriEnd"
                    }
                });
            } catch (Exception ex)
            {
                SendWebHocErrorkMessage(ex.Message);
                UTCTimeLog(ex.Message);
            }

        }).Start();
    }
    public static async void SendWebHockMessageUpdate(string text)
    {
        new Thread(() =>
        {
            try
            {
                var config = TradingView.GetConfigJson();
                WebClient msgstart = new WebClient();
                msgstart.UploadValues(config.UpdatesWebhock, new NameValueCollection
                {
                    {
                        "content",
                        text ?? ""
                    },
                    {
                        "username",
                        "FriEnd"
                    }
                });
            } catch(Exception ex)
            {
                SendWebHocErrorkMessage(ex.Message);
                UTCTimeLog(ex.Message);
            }

        }).Start();
    }
    public static async void SendWebHocErrorkMessage(string text)
    {
        new Thread(() =>
        {
            try
            {
                var config = TradingView.GetConfigJson();
                WebClient msgstart = new WebClient();
                msgstart.UploadValues(config.ErrorsWebhock, new NameValueCollection
                {
                    {
                        "content",
                        text ?? ""
                    },
                    {
                        "username",
                        "FriEnd"
                    }
                });
            } catch (Exception ex)
            {
                UTCTimeLog(ex.Message);
            }
        }).Start();
    }
    public static void Log(string text)
    {
        Console.WriteLine(text);
        try
        {
            File.AppendAllText($"{Root_Path()}/Data/Logs.txt", text + "\n");
        } catch(Exception ex)
        {
            File.AppendAllText($"{Root_Path()}/Data/Logs2.txt", text + "\n");
        }
        
    }
    public static void UTCTimeLog(string text)
    {
        Console.WriteLine($"{DateTime.UtcNow}: " + text);
        try
        {
            File.AppendAllText($"{Root_Path()}/Data/Logs.txt", $"{DateTime.UtcNow}: " + text + "\n");
        }
        catch (Exception ex)
        {
            File.AppendAllText($"{Root_Path()}/Data/Logs2.txt", $"{DateTime.UtcNow}: " + text + "\n");
        }
        
    }
    public static decimal Get_Untel_Two(decimal num)
    {
        string str_num = $"{num}";
        if (!$"{num}".Contains("."))
        {
            return num;
        }
        var final = "";
        var is_real_number = false;
        var i = 0;
        foreach(char number in str_num.Split(".")[1])
        {
            if(i == 0)
            {
                if (is_real_number)
                {
                    final += number;
                    i++;
                    continue;
                }
                if (number == '0')
                {
                    final += number;
                    continue;
                }
                if (number != '0')
                {
                    final += number;
                    is_real_number = true;
                }
            } else
            {
                break;
            }
        }
        return decimal.Parse($"{str_num.Split(".")[0]}.{final}") - (decimal)0.01;
    }
}