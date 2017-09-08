using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreIdentity2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreIdentity2.Pages.Account;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AspNetCoreIdentity2.Controllers.api
{
    [Produces("application/json")]
    [Route("api/token")]
    public class TokenController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public TokenController(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        /// <summary>
        /// Generates the token.
        /// </summary>
        /// <returns>The token.</returns>
        /// <param name="model">Email : email, Passwod : passworkd</param>
        /// <remarks>
        /// [FromBody] => http body 에 json 데이터 형식으로 전달되는 경우 
        /// [FromForm] => http form 데이터 형식으로 전달되는 경우
        /// </remarks>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> GenerateToken(/*[FromForm]*/[FromBody]LoginModel.InputModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                var result =  await _userManager.CheckPasswordAsync(user, model.Password);
                if (result == true)
                {
                    var now = DateTime.UtcNow;

                    // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
                    // You can add other claims here, if you want:
                    //var claims = new List<Claim>
                    //{
                    //    new Claim(JwtRegisteredClaimNames.Sub, username),
                    //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    //    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
                    //};

                    var userClaims = await _userManager.GetRolesAsync(user);
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                    claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                    claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64));
                    //claims.AddRange(user.Claims.ToArray());

                    foreach (var x in userClaims)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, x));
                    }

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("TokenAuthentication:SecretKey").Value));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var expries = TimeSpan.FromMinutes(3600);
                    var jwt = new JwtSecurityToken(
                        issuer: _config.GetSection("TokenAuthentication:Issuer").Value,
                        audience: _config.GetSection("TokenAuthentication:Audience").Value,
                        claims: claims,
                        notBefore: now,
                        expires: now.Add(expries),
                        signingCredentials: creds);
                    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                    var response = new
                    {
                        access_token = encodedJwt,
                        expires_in = (int)expries.TotalSeconds,
                        expires = now.Add(expries),
                    };

                    return Ok(response);
                }
            }

            return BadRequest("Could not create token");
        }
    }
}
