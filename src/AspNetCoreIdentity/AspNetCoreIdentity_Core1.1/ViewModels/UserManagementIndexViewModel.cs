using System;
using System.Collections.Generic;
using AspNetCoreIdentity.Models;

namespace AspNetCoreIdentity.ViewModels
{
    public class UserManagementIndexViewModel
    {
        
        public List<ApplicationUser> Users { get; set; }
    }
}
