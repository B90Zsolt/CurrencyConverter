using CurrencyConverter.Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;


namespace CurrencyConverter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyLogic currencyLogic;

        public CurrencyController(ICurrencyLogic currencyLogic)
        {
            this.currencyLogic = currencyLogic;
        }

        /// <summary>
        /// MNB's currencies with current exchange rates
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /currency
        ///
        /// </remarks>
        /// <returns>The currencies</returns>
        /// <response code="200">Returns the currencies</response>
        /// <response code="400">Failed to collect data</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Models.ExchangeRates))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCurrencies() 
        {
            var result = await currencyLogic.GetCurrenciesAsync();

            if (result == null)
                return BadRequest("Query failed");

            return Ok(result);
        }
    }
}
