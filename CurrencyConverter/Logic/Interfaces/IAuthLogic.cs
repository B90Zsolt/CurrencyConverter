using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CurrencyConverter.Logic.Interfaces
{
    public interface IAuthLogic
    {
        Task<IEnumerable<Claim>> CreateUserAsync(string email, string password);
        Task<IEnumerable<Claim>> LoginAsync(string email, string password);
        Task<IEnumerable<Claim>> RenewTokenAsync(string currentToken);
    }
}
