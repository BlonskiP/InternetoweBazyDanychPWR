using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskRegiser.Core;
using TaskRegiser.Core.Entities;
using TaskRegiser.ModelView;

namespace TaskRegister.View.Controllers
{
    [Authorize(Roles = RolesResource.Policy.AdminOnly)]
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<Employee> _userManager;
        public EmployeeController(AppDbContext context, UserManager<Employee> userManager)
        {
            _dbContext = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var appDbContext = await _dbContext.Users.ToListAsync();
            return View(appDbContext);
        }

        public async Task<IActionResult> Edit(string? id)
        {
            ViewBag.id = id;
            if (id == null)
            {
                return NotFound();
            }
            Employee user = await _dbContext.Users.FindAsync(id);
            
            if (user == null)
            {
                return NotFound();
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            ViewData["Roles"] = userRoles.ToList();
            return View(user);
        }
        public async Task<IActionResult> ManageUserRoles(string? userId)
        {
            ViewBag.userId = userId;
            var user = await _dbContext.Users.FindAsync(userId);
            if(user==null)
            {
                return NotFound();
            }
            var model = new List<UserRolesViewModel>();
            foreach(var role in await _dbContext.Roles.ToListAsync())
            {
                var userRoleViewModel = new UserRolesViewModel()
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.isSelected = true;
                }
                else
                    userRoleViewModel.isSelected = false;

                model.Add(userRoleViewModel);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user==null)
            {
                ViewBag.ErrorMessage = "User not found";
                return View("NotFound");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "Canno delete older roles");
                return View(model);
            }

            result = await _userManager.AddToRolesAsync(user, model.Where(modelRole=>modelRole.isSelected).Select(y=>y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Canno add new roles");
                return View(model);
            }
            return RedirectToAction("Edit", new { Id = userId });
        }
    }
}
