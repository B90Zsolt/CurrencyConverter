using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyConverter.Models
{
    public class ExchangeRates
    {
        public DateTime Date { get; set; }

        public IEnumerable<Rate> Rates { get; set; }
    }
}
