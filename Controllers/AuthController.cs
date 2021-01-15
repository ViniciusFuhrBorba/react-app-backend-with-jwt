using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Conectadev.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System;
using Microsoft.Extensions.Options;



namespace Conectadev.Api.Controllers
{
    //https://localhost:5001/v1/auth
    [Route("v1/auth")]
    public class AuthController : Controller
    {
        private readonly JWTSettings _jwtsettings;

        public AuthController(IOptions<JWTSettings> jwtsettings)
        {
            _jwtsettings = jwtsettings.Value;
        }


        //https://localhost:5001/v1/auth/login
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserToken>> Authenticate([FromBody] User model)
        {

            var user = new User
            {

                id = 1,
                Name = "Vinicius",
                Username = "vinicius",
                Email = "vinicius@gmail.com",
                Avatar = "/images/avatars/avatar_1.jpeg"
            };

            if (user == null)
            {
                return NotFound(new { message = "Email ou senha inválidos" });
            }

            var token = GenerateAccessToken(user);

            return new UserToken
            {
                User = user,
                Token = token
            };

        }
            [HttpGet]
            [Route("anonymous")]
            [AllowAnonymous]

            public string Anonymous() => "Anônimo";

            [HttpGet]
            [Route("authenticated")]
            [Authorize]

            public string Authenticated() => "Autenticado";
        private string GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtsettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Email.ToString()),
            }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}