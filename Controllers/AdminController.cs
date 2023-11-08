using BudgetManager.ViewModels;
using BudgetManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;



namespace BudgetManager.Controllers
{
    [ServiceFilter(typeof(LogAttribute))]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public IActionResult RoleManagement()
        {
            var usersWithRoles = _userManager.Users
                .Select(user => new UserWithRolesViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Roles = _userManager.GetRolesAsync(user).Result
                })
                .ToList();
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            ViewBag.AllRoles = allRoles; // Pass all roles to the view

            return View(usersWithRoles);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public IActionResult AssignRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Handle the case where the user is not found
                return RedirectToAction("RoleManagement");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Remove current roles
            var result = await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!result.Succeeded)
            {
                // Handle errors if needed
            }

            // Assign the selected role
            result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                // Role assigned successfully
                return RedirectToAction("RoleManagement");
            }
            else
            {
                // Handle errors
                foreach (var error in result.Errors)
                {
                    // Add error handling code (e.g., logging, displaying errors, etc.)
                }
                return RedirectToAction("RoleManagement");
            }
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> RemoveRole(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                // Handle invalid input
                return RedirectToAction("RoleManagement");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Handle not found user
                return RedirectToAction("RoleManagement");
            }

            // Remove the user
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                // User deleted successfully
                return RedirectToAction("RoleManagement");
            }
            else
            {
                // Handle errors
                foreach (var error in result.Errors)
                {
                    // Add error handling code (e.g., logging, displaying errors, etc.)
                }
                return RedirectToAction("RoleManagement");
            }
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin, Master")]
        public async Task<IActionResult> SearchUsers(string searchPhrase)
        {
            if (searchPhrase == null)
            {
                var usersWithRoles = _userManager.Users
                    .Select(user => new UserWithRolesViewModel
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        Roles = _userManager.GetRolesAsync(user).Result
                    })
                    .ToList();
                var allRole = _roleManager.Roles.Select(r => r.Name).ToList();

                ViewBag.AllRoles = allRole; // Pass all roles to the view

                return View("RoleManagement", usersWithRoles);
            }
            // Search for records that match the "UserName" column
            var user = await _userManager.FindByEmailAsync(searchPhrase);
            var userWithRoles = user != null
                ? new List<UserWithRolesViewModel>
                {
            new UserWithRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = _userManager.GetRolesAsync(user).Result
            }
                }
                : new List<UserWithRolesViewModel>();

            var allRoles = user != null
                ? _roleManager.Roles.Select(r => r.Name).ToList()
                : new List<string>();

            ViewBag.AllRoles = allRoles; // Pass all roles to the view
            return View("RoleManagement", userWithRoles);

        }





    }
}