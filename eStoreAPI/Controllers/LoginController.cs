using BusinessObject.Models;
using DataAccess.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eStoreAPI.Controllers
{
    
    [Route("api/login")]
    [ApiController]
    public class LoginController : ODataController
    {
        // GET: api/<LoginController>
        private readonly Member _adminAccount;
        private readonly IConfiguration _config;
        private readonly JwtOption _jwtOption;
        private readonly IUnitOfWork _unitOfWork;
        private class JwtOption
        {
            public string? Key { get; set; }
            public string? Issuer { get; set; }
            public string? Audience { get; set; }
        }
        public LoginController(IConfiguration config, IUnitOfWork unitOfWork)
        {
            
            _config = config;
            _unitOfWork = unitOfWork;
            _adminAccount = new Member()
            {
                Email = _config.GetSection("eStoreAccount")["Email"],
                Password = _config.GetSection("eStoreAccount")["Password"]
            };
            _jwtOption = new JwtOption()
            {
                Key = _config.GetSection("Jwt:Key").Value,
                Issuer = _config.GetSection("Jwt:Issuer").Value,
                Audience = _config.GetSection("Jwt:Audience").Value,
            };
        }

        [HttpGet]
        public async Task<IActionResult> Get(string? token)
        {
            try
            {
                if (token == null)
                {
                    return BadRequest();
                }
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                string email = jwtToken.Payload.Claims.First(claim => claim.Type == "email").Value;
                if(email == _adminAccount.Email)
                {
                    return Ok(_adminAccount);
                } else
                {
                    string role = jwtToken.Payload.Claims.First(claim => claim.Type == "role").Value;
                    var find = await _unitOfWork.MemberRepository.Get(predicate: m => m.Email == email);
                    var login = find.FirstOrDefault();
                    return Ok(login);
                }
                
            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            
        }


        // POST api/<LoginController>
        [HttpPost]
        public async Task<IActionResult> Post([FromQuery] string email, [FromQuery] string password)
        {
            //try login as Administrator
            if(email == _adminAccount.Email && password == _adminAccount.Password)
            {
                var claims = new Claim[]
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, "Administrator"),
                    new Claim("id", "0")
                };
                return Ok(GenerateToken(claims));
            } else
            {
                var login = await _unitOfWork.MemberRepository.LoginAsync(email, password);
                if(login != null)
                {
                    var claims = new Claim[]
                    {
                        new Claim("id", login.MemberId.ToString()),
                        new Claim(ClaimTypes.Email, login.Email),
                        new Claim("country", login.Country),
                        new Claim(ClaimTypes.Role, "Member"),
                        new Claim("city", login.City),
                        new Claim("company", login.CompanyName)
                    };
                    return Ok(GenerateToken(claims));
                } else
                {
                    return Unauthorized();
                }
            }            
        }

        private string GenerateToken(Claim[] claims)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                IssuedAt = DateTime.Now,
                Issuer = _jwtOption.Issuer,
                Audience = _jwtOption.Audience,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOption.Key)), 
                    SecurityAlgorithms.HmacSha256Signature
                    )
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);
            return token;
        }
    }
}
