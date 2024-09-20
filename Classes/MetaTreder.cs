using Newtonsoft.Json;
using trading_bot_3.MetaModels;
namespace trading_bot_3.Classes
{
    public class MetaTreder
    {
        public OpenOrder Command(string command)
        {
            Client v = new Client();
                var res = v.Command(command);//1,BUY,2,2700,2500
            Console.WriteLine(res);
                var x=  JsonConvert.DeserializeObject<OpenOrder>(res);
            return x;
            
        }
    }
}
