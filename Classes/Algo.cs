namespace trading_bot_3;

public class Algo{
    public static void Run(){
        TradingView.Login();
        TradingView.My_Chart_Window.OpenChart();
        //TradingView.My_Chart_Window.RemoveAllIndectors();
        //TradingView.My_Chart_Window.Open_Buy_Sell_Indicator();
        Fn.Sleep(3);
        TradingView.My_Chart_Window.Chose_Time();
        //TradingView.My_Chart_Window.Click_Data_Windwo();
        if (!TradingView.My_Chart_Window.Is_Plot_Elemnt_found())
        {
            throw new Exception("Plot Console Not Found !!");
        }
        TradingView.My_Chart_Window.Algo();
        Fn.SendWebHockMessageUpdate($"***Bot Started with ({TradingView.My_Chart_Window.Money}) :dollar:***\n**coin ({TradingView.My_Chart_Window.ChartName.ProgramName}) :white_check_mark:**\n*{DateTime.UtcNow}*");
        Fn.UTCTimeLog($" | :) | Start Trading ({TradingView.My_Chart_Window.ChartName.ProgramName}) cOin mY FriEnd...");
    }
}
