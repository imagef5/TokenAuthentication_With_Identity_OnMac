using AspNetCoreIdentity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AspNetCoreIdentity.Controllers
{
    [Authorize(Roles = "Member")]
    //[Route("Members/[controller]")]
    public class MemberHomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public MemberHomeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            var claims = User.Claims;
            return View();
        }

        public IActionResult AccessGranded()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult AnonymousAccess()
        {
            return View();
        }
    }
}
