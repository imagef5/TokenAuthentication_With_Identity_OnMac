using System;
using System.Threading.Tasks;
using AspNetCoreIdentity.Middleware.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using AspNetCoreIdentity.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace AspNetCoreIdentity.Middleware
{
    public class TokenProviderMiddleware
    {
		private readonly RequestDelegate _next;
		private readonly TokenProviderOptions _options;
        private readonly UserManager<ApplicationUser> _userManager;

		public TokenProviderMiddleware(
			RequestDelegate next,
			IOptions<TokenProviderOptions> options,
            UserManager<ApplicationUser> userManager)
		{
			_next = next;
			_options = options.Value;
            _userManager = userManager;
		}

		public Task Invoke(HttpContext context)
		{
			// If the request path doesn't match, skip
			if (!context.Request.Path.Equals(_options.Path, StringComparison.Ordinal))
			{
				return _next(context);
			}

			// Request must be POST with Content-Type: application/x-www-form-urlencoded
			if (!context.Request.Method.Equals("POST") || !context.Request.HasFormContentType)
			{
				context.Response.StatusCode = 400;
				return context.Response.WriteAsync("Bad request.");
			}

			return GenerateToken(context);
		}

		private async Task GenerateToken(HttpContext context)
		{
			var username = context.Request.Form["username"];
			var password = context.Request.Form["password"];

			var identity = await GetIdentity(username, password);
            //ApplicationUser user = null;
            //user = await _userManager.FindByNameAsync(username);
            //var result = await _userManager.CheckPasswordAsync(user, password);

            if (identity == null)
			{
				context.Response.StatusCode = 400;
				await context.Response.WriteAsync("Invalid username or password.");
				return;
			}

			var now = DateTime.UtcNow;

            //var userClaims = await _userManager.GetRolesAsync(user);

			// Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
			// You can add other claims here, if you want:
            var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, username),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
			};

            claims.AddRange(identity.Claims);

			var jwt = new JwtSecurityToken(
				issuer: _options.Issuer,
				audience: _options.Audience,
				claims: claims,
				notBefore: now,
				expires: now.Add(_options.Expiration),
				signingCredentials: _options.SigningCredentials);
			var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

			var response = new
			{
				access_token = encodedJwt,
				expires_in = (int)_options.Expiration.TotalSeconds,
                expires = now.Add(_options.Expiration),
			};

			// Serialize and return the response
			context.Response.ContentType = "application/json";
			await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
		}

        private async Task<ClaimsIdentity> GetIdentity(string username, string password)
		{
			ApplicationUser user = null;
			user = await _userManager.FindByNameAsync(username);
			var result = await _userManager.CheckPasswordAsync(user, password);
            if (result)
            {
                var claimIdentity = new ClaimsIdentity(new System.Security.Principal.GenericIdentity(username, "Token"), new Claim[] { });
                var userClaims = await _userManager.GetRolesAsync(user);
				foreach (var x in userClaims)
				{
                    claimIdentity.AddClaim(new Claim(ClaimTypes.Role, x));
				}
                return claimIdentity;
            }
			// Credentials are invalid, or account doesn't exist
            return null;
		}
    }
}
