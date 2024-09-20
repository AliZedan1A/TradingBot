using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trading_bot_3.MetaModels
{
    public class CloseOrder
    {
        public bool Sus { get; set; }
        public double PriceOpen { get; set; }
        public double Tp { get; set; }
        public double Sl { get; set; }
        public double OrderTicket { get; set; }

    }
}
