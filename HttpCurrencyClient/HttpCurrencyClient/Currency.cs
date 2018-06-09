using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpCurrencyClient
{
    public class Currency
    {
        public string Symbol { get; set; }
        public float Price { get; set; }
        public float Bid { get; set; }
        public float Ask  {get; set; }
        public float Timestamp { get; set; }
    }
}
