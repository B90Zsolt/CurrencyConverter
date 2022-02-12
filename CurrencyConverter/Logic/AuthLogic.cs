using CurrencyConverter.EntityFramework;
using CurrencyConverter.Logic.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CurrencyConverter.Logic
{

    public class AuthLogic : IAuthLogic
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<AuthLogic> logger;

        public AuthLogic(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AuthLogic> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        public async Task<IEnumerable<Claim>> LoginAsync(string email, string password)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                    return null;

                SignInResult result = await signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: true);
                if (!result.Succeeded)
                {
                    logger.LogWarning("Error: {0}: Incorrect email or password: {1}", "LoginAsync()", email);
                    return null;
                }

                return GetClaims(user);
            }
            catch (Exception e)
            {
                logger.LogError("Exception in LoginAsync(): " + e.Message);
                return null;
            }
        }

        public async Task<IEnumerable<Claim>> CreateUserAsync(string email, string password)
        {
            try
            {
                var existingUser = await userManager.FindByEmailAsync(email);
                if(existingUser != null)
                {
                    logger.LogWarning("Error: {0}: User already exists: {1}", "CreateUserAsync()", email);
                    return null;
                }

                var result = await userManager.CreateAsync(new ApplicationUser { Email = email, UserName = email }, password);
                if (!result.Succeeded)
                {
                    logger.LogWarning("Error: {0}: Failed creating user: {1}", "CreateUserAsync()", email);
                    
                    return null;
                }

                return await LoginAsync(email, password);
            }
            catch (Exception e)
            {
                logger.LogError("Exception in CreateUserAsync(): " + e.Message);
                return null;
            }
        }

        public async Task<IEnumerable<Claim>> RenewTokenAsync(string currentToken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(currentToken);
                var email = jwt.Claims.First(f => f.Type == ClaimTypes.Email).Value;

                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                    return null;

                return GetClaims(user);
            }
            catch (Exception e)
            {
                logger.LogError("Exception in RenewTokenAsync(): " + e.Message);
                return null;
            }
        }
        
        private IEnumerable<Claim> GetClaims(ApplicationUser user)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Email, user.Email));

            return claims;
        }
    }
}
