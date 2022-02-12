using CurrencyConverter.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CurrencyConverter.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace CurrencyConverter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IAuthLogic authLogic;
        private readonly IConfiguration config;

        public UserController(IAuthLogic authLogic, IConfiguration config)
        {
            this.authLogic = authLogic;
            this.config = config;
        }

        /// <summary>
        /// User login
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /user/login
        ///     {
        ///        "email": "example@example.hu",
        ///        "password": Password1234
        ///     }
        ///
        /// </remarks>
        /// <param name="data"></param>
        /// <returns>JWT token</returns>
        /// <response code="200">Returns a new JWT</response>
        /// <response code="401">Login failed</response>  
        /// <response code="400">Validation failed</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] User data)
        {
            if (ModelState.IsValid)
            {
                var claims = await authLogic.LoginAsync(data.Email, data.Password);

                if (claims == null)
                    return Unauthorized("Login failed");

                return Ok(BuildToken(claims));
            }
            return ValidationProblem();
        }

        /// <summary>
        /// User registration
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /user/registration
        ///     {
        ///        "email": "example@example.hu",
        ///        "password": Password1234
        ///     }
        ///
        /// </remarks>
        /// <param name="data"></param>
        /// <returns>JWT token</returns>
        /// <response code="200">Returns a new JWT</response>
        /// <response code="400">Registration failed</response>
        [HttpPost("registration")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Registration([FromBody] User data)
        {
            if (ModelState.IsValid)
            {
                var claims = await authLogic.CreateUserAsync(data.Email, data.Password);

                if (claims == null)
                    return BadRequest("Registration failed");

                return Ok(BuildToken(claims));
            }
            return ValidationProblem();
        }

        /// <summary>
        /// Renew JWT
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /user/renew-token
        ///
        /// </remarks>
        /// <returns>JWT token</returns>
        /// <response code="200">Returns a new JWT token</response>
        /// <response code="400">Renew failed</response>
        [HttpGet("renew-token")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RenewToken()
        {
            var claims = await authLogic.RenewTokenAsync(AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]).Parameter);

            if (claims == null)
                return BadRequest("Renew failed");

            return Ok(BuildToken(claims));
        }

        private string BuildToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(config["Jwt:Issuer"],
                                             config["Jwt:Issuer"],
                                             claims,
                                             expires: DateTime.Now.AddMinutes(int.Parse(config["Jwt:Expires"])),
                                             signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
