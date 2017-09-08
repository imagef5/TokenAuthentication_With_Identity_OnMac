using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreIdentity.Models;
using AspNetCoreIdentity.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AspNetCoreIdentity.Controllers
{
    public class MembersController : Controller
    {
		private readonly SignInManager<ApplicationUser> _singInManager;

		public MembersController(SignInManager<ApplicationUser> signInManager)
		{
			_singInManager = signInManager;
		}

		[HttpGet]
		public IActionResult Login()
		{
			ViewBag.Title = "Login Page";
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel vm)
		{
			if (ModelState.IsValid)
			{
				var result = await _singInManager.PasswordSignInAsync(vm.Email, vm.Password, vm.RememberMe, false);
				if (result.Succeeded)
				{
					return RedirectToAction("Index", "Home");
				}
				ModelState.AddModelError("", "Invalid login attempt.");
			}
			return View(vm);
		}

		public IActionResult AccessDenied()
		{
			return View();
		}
    }
}
