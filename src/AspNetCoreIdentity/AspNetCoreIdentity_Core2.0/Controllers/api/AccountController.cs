using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using AspNetCoreIdentity2.Data;
using AspNetCoreIdentity2.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AspNetCoreIdentity2.Controllers.api
{
    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public AccountController(ApplicationDbContext db)
        {
            this._dbContext = db;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public IActionResult get()
        {

            var obj2 = this.GetDbUser(_dbContext);

            return Content(JsonConvert.SerializeObject(new { _data = obj2 }, Formatting.Indented), "application/json");
        }
        

    }
}