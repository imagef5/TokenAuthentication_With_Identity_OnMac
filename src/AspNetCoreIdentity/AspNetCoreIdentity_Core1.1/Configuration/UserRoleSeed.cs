using System;
using System.Threading.Tasks;
using AspNetCoreIdentity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AspNetCoreIdentity.Configuration
{
    public class UserRoleSeed
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRoleSeed(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
           //context.Roles.Add(new IdentityRole{ Name = "Admin"});
        }

        public async void Seed()
        {
            string[] roles = new[] { "Member", "Admin", "Employee" };

			foreach (var role in roles)
			{
				if (!await _roleManager.RoleExistsAsync(role))
				{
					var newRole = new IdentityRole(role);
					await _roleManager.CreateAsync(newRole);
				}
			}
        }
    }
}
