namespace trading_bot_3.MetaModels
{
    public class OpenOrder //1,Buy,LOT,tp,sl
    {
        public bool Sus { get; set; }
        public double PriceOpen { get; set; }
        public double Tp { get; set; }
        public double Sl { get; set; }
        public double OrderTicket { get; set; }
        public string Comment { get; set; }
    }
}
