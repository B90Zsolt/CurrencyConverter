using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyConverter.Models
{
    public class Rate
    {
        public string Name { get; set; }
        public decimal Unit { get; set; }
        public decimal Value { get; set; }
    }
}
