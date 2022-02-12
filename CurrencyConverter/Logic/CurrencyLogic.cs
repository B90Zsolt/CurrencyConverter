using CurrencyConverter.Logic.Interfaces;
using CurrencyConverter.Models;
using Microsoft.Extensions.Logging;
using MNBService;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CurrencyConverter.Logic
{
    public class CurrencyLogic : ICurrencyLogic
    {
        private readonly ILogger<CurrencyLogic> logger;
        private readonly MNBArfolyamServiceSoap client;
        public CurrencyLogic(ILogger<CurrencyLogic> logger, MNBArfolyamServiceSoap client)
        {
            this.logger = logger;
            this.client = client;
        }

        public async Task<ExchangeRates> GetCurrenciesAsync()
        {
            try
            {
                var r = await client.GetCurrentExchangeRatesAsync(new GetCurrentExchangeRatesRequest());
                var mnb = DeserializeResult<MNBCurrentExchangeRates>(r.GetCurrentExchangeRatesResponse1.GetCurrentExchangeRatesResult);
                
                return new ExchangeRates
                {
                    Date = DateTime.Parse(mnb.Day.date),
                    Rates = mnb.Day.Rate.Select(s => new Models.Rate
                    {
                        Name = s.curr,
                        Unit = decimal.Parse(s.unit),
                        Value = decimal.Parse(s.Text[0])
                    })
                };
            }
            catch(Exception e)
            {
                logger.LogError("Exception in GetCurrenciesAsync(): " + e.Message);
                return null;
            }
        }

        private T DeserializeResult<T>(string result)
        {
            using (var reader = new StringReader(result))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}
