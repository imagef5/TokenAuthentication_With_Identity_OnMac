using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreApi.DataServices;
using AspNetCoreApi.DataServices.Interfaces;
using AspNetCoreApi.Configures;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCoreApi
{
    public class Startup
    {
		//개발 환경용으로 세팅됨 
		private static readonly string secretKey = "mysupersecret_secretkey!123";
		private static readonly string issure = "ExampleIssuer";
		private static readonly string audience = "ExampleAudience";

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
			// Adds services required for using options.
			services.AddOptions();

			// Register the IConfiguration instance which MyOptions binds against.
			services.Configure<UsersApiServiceOptions>(Configuration);

			// Add framework services.
			services.AddMvc();

			// Register application services.
            services.AddScoped<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

            #region Token Validation Setting
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
			//generate token

			var tokenValidationParameters = new TokenValidationParameters
			{
				// The signing key must match!
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = signingKey,

				// Validate the JWT Issuer (iss) claim
				ValidateIssuer = true,
				ValidIssuer = issure,

				// Validate the JWT Audience (aud) claim
				ValidateAudience = true,
				ValidAudience = audience,

				// Validate the token expiry
				ValidateLifetime = true,

				// If you want to allow a certain amount of clock drift, set that here:
				ClockSkew = TimeSpan.Zero
			};

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
				TokenValidationParameters = tokenValidationParameters
			});
            #endregion

            app.UseMvc();
        }
    }
}
