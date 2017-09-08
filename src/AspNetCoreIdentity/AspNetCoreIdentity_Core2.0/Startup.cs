using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreIdentity2.Data;
using AspNetCoreIdentity2.Middleware;
using AspNetCoreIdentity2.Middleware.DataModels;
using AspNetCoreIdentity2.Models;
using AspNetCoreIdentity2.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCoreIdentity2
{
    public class Startup
    {
        //개발 환경용으로 세팅됨 
        //private static readonly string secretKey = "mysupersecret_secretkey!123";
        private static string issure; //= "ExampleIssuer";
        private static string audience; // = "ExampleAudience";

        private static SymmetricSecurityKey signingKey;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("TokenAuthentication:SecretKey").Value));
            issure = Configuration.GetSection("TokenAuthentication:Issuer").Value;
            audience = Configuration.GetSection("TokenAuthentication:Audience").Value;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                                                       options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>(
                    o =>
                    {
                        o.Password.RequireDigit = false;
                        o.Password.RequiredLength = 0;
                        o.Password.RequireLowercase = false;
                        o.Password.RequireNonAlphanumeric = false;
                        o.Password.RequireUppercase = false;

                    })
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            //services.ConfigureApplicationCookie(options => options.LoginPath = "/Account/Login");

            var tokenValidationParameters = new TokenValidationParameters
            {
                //The signing key must match !
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                //Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = issure,

                //validate the JWT Audience (aud) claim

                ValidateAudience = true,
                ValidAudience = audience,

                //validate the token expiry
                ValidateLifetime = true,

                // If you  want to allow a certain amout of clock drift
                ClockSkew = TimeSpan.Zero
            };

            #region 인증설정
            // Jwt 인증과 Cookie 인증 동시에 사용할 경우 
            services.AddAuthentication(o =>
                    {
                        //o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //CookieAuthenticationDefaults.AuthenticationScheme;
                        //o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; //CookieAuthenticationDefaults.AuthenticationScheme; 
                    })
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = tokenValidationParameters;
                    })
                    .AddCookie(options =>
                    {
                        //options.Cookie.Name = Configuration.GetSection("TokenAuthentication:CookieName").Value;
                        options.ExpireTimeSpan = TimeSpan.FromDays(150);
                        options.LoginPath = "/Account/Login";
                        options.LogoutPath = "/Account/LogOff";
                        //options.TicketDataFormat = new CustomJwtDataFormat(
                                                            //SecurityAlgorithms.HmacSha256,
                                                            //tokenValidationParameters);
                        //options.SlidingExpiration = true;
                    });
            #endregion

            services.AddMvc()
                   .AddRazorPagesOptions(options =>
                   {
                       options.Conventions.AuthorizeFolder("/Account/Manage");
                       options.Conventions.AuthorizePage("/Account/Logout");
                   });

            // Register no-op EmailSender used by account confirmation and password reset during development
            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
            services.AddSingleton<IEmailSender, EmailSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            //.Net Core 2.0 인증 -> UseAuthentication Method 로 통합
            app.UseAuthentication();

            #region TokenProvider Middleware Setting
            //미들웨어를 통한 토큰 발행 
            var jwtOptions = new TokenProviderOptions
            {
                Path = Configuration.GetSection("TokenAuthentication:TokenPath").Value,
                Audience = audience,
                Issuer = issure,
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            };
            app.UseJWTTokenProviderMiddleware(Options.Create(jwtOptions));
            //컨트롤러를 통한 토큰 발행 -> /Controllers/Api/TokenController.cs 참조 
            //controller 사용시 위 코드 주석 처리
            #endregion

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
