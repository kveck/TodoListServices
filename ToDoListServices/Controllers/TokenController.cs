namespace ToDoListServices.Controllers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using ToDoListServices.Common.ErrorHandling;
    using ToDoListServices.Data.Dto;

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;

        public TokenController(IConfiguration config, ILogger<TokenController> logger)
        {
            Guard.NotNull(logger, nameof(logger));
            Guard.NotNull(config, nameof(config));

            _config = config;
            _logger = logger;
        }

        /// <summary>
        /// public method to create JWT for valid user
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public IActionResult CreateToken([FromBody] LoginDto login)
        {
            var user = Authenticate(login);

            if (user == null)
                return Unauthorized();

            var tokenString = BuildToken(user);
            return Ok(new {token = tokenString});
        }

        /// <summary>
        /// authenticate user
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        private UserDto Authenticate(LoginDto login)
        {
            // hard-coded for sample code - normally would involve application authentication
            if (login.Username == "superuser" && login.Password == "password")
            {
                return new UserDto { Name = "SuperUSer", Email = "superuser@domain.com" };
            }

            return null;
        }

        /// <summary>
        /// build JWT token for valid user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string BuildToken(UserDto user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // build claims from user info
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Name ),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"], //issuer
                _config["Jwt:Issuer"], //audience
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}