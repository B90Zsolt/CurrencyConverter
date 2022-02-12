using CurrencyConverter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyConverter.Logic.Interfaces
{
    public interface ICurrencyLogic
    {
        Task<ExchangeRates> GetCurrenciesAsync();
    }
}
