namespace trading_bot_3;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
public class Bot
{
    public static bool Is_Bot_Running = false;
    public static int Curent_page_index = 0;
    public static WebDriver Driver = new ChromeDriver();
    public static void Start()
    {
        if(Is_Bot_Running){
            return;
        }
        Driver.Url = "https://google.com";
        Is_Bot_Running = true;
    }
    public static void ScreenShut(string name)
    {
        new Thread(() =>
        {
            try
            {
                var img = ((ITakesScreenshot)Driver).GetScreenshot();
                img.SaveAsFile($"{Fn.Root_Path()}/Data/Imgs/{name.Replace("\\", "_").Replace("/", "_").Replace(":", "-")}.png");
            } catch(Exception ex)
            {
                Fn.UTCTimeLog(ex.Message);
            }

        }).Start();

    }
    public static void SetUrl(string url){
        Driver.Url = url;
    }
    public static void OpenWindow(string url)
    {
        Driver.ExecuteScript($"window.open('{url}')");
        Curent_page_index += 1; 
        var newPage = Driver.WindowHandles[Curent_page_index];
        Driver.SwitchTo().Window(newPage);
    }
    public static void SwitchWindowByIndex(int index)
    {
        var page = Driver.WindowHandles[index];
        Driver.SwitchTo().Window(page);
        Curent_page_index = index;
    }
    public static void GetGetUrl(int index)
    {
        var page = Driver.WindowHandles[index];
        Driver.SwitchTo().Window(page);
        Curent_page_index = index;
    }
    public static string GetCurrentURL(){
       return (String) Driver.ExecuteScript("return window.location.href");
    }
    public static IWebElement QuerySelector(string selector){
        return Driver.FindElement(By.CssSelector(selector));
    }
    public static List<IWebElement> QuerySelectorAll(string selector){
        return [.. Driver.FindElements(By.CssSelector(selector))];
    }
    
    public static void Close()
    {
        if(!Is_Bot_Running){
            return;
        }
        Driver.Quit();
        Is_Bot_Running = false;
    }
}