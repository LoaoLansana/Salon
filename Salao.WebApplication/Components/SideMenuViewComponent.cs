using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Salao.Repository.Models;
using Salao.Repository.Models.Account;
using Salao.Repository.Models.Menus.View;

namespace Salao.WebApplication.Components
{
    public class SideMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SideMenuViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return View(new MenuRoleViewModel[0]);
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var menusWithRoles = await _context.Menus
                .Include(m => m.MenuRoles)
                .ThenInclude(mr => mr.Role)
                .Where(menu => menu.MenuRoles.Any(mr => userRoles.Contains(mr.Role.Name)))
                .OrderBy(x => x.Name)
                .ToListAsync();

            var viewModel = menusWithRoles.Select(menu => new MenuRoleViewModel
            {
                Id = menu.Id,
                Name = menu.Name,
                Icon = menu.Icon,
                Route = menu.Route,
                ParentMenuId = menu.ParentMenuId,
                Roles = menu.MenuRoles.Select(mr => new MenuRoleModel
                {
                    RoleId = mr.RoleId,
                    RoleName = mr.Role.Name
                }).ToList()
            }).ToList();

            return View(viewModel);
        }
    }
}
