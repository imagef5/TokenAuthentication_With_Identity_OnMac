using AspNetCoreIdentity.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AspNetCoreIdentity.ViewModels
{
    public class UserManagementAddRoleViewModel
    {
        public ApplicationUser User { get; set; }

        public string NewRole { get; set; }
        //public IdentityRole Role { get; set; }

        public SelectList Roles { get; set; }
    }
}
