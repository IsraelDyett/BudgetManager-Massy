using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

public class RoleManagementViewModel
{
    public List<IdentityUser> Users { get; set; }
    public List<IdentityRole> Roles { get; set; }
}
