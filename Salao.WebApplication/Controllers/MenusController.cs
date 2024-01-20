
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Salao.Repository.Models;
using Salao.WebApplication.Extensions;
using Salao.Repository.Models;
using Salao.Repository.Models.Menus.View;
using AutoMapper;

namespace Reanvam.App.Controllers
{
    public class MenusController : BaseController
    {
        #region Propriedades
        private readonly ApplicationDbContext _context;
        private IMapper _mapper;
        #endregion

        #region Construtores
        public MenusController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        #endregion

        #region Métodos Públicos
        [HttpGet]
        public ActionResult Index()
        {
            var menus = _context.Menus.AsQueryable().OrderBy(x => x.Name).ToList();

            return View(menus);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var roles = _context.Roles.AsQueryable();
            var menus = _context.Menus.AsQueryable();

            ViewBag.Roles = roles;
            ViewBag.Menus = menus;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuViewModel menuModel)
        {
            if (ModelState.IsValid)
            {
                Menu menu = _mapper.Map<Menu>(menuModel);
                menu.CreatedAt = DateTime.UtcNow;
                menu.UpdatedAt = DateTime.UtcNow;
                _context.Add(menu);

                await _context.SaveChangesAsync();

                var RoleIds = Request.Form["chkRoles"];

                foreach (var roleId in RoleIds)
                {
                    MenuRole menuRole = new MenuRole();
                    menuRole.RoleId = roleId;
                    menuRole.MenuId = menu.Id;
                    _context.Add(menuRole);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(menuModel);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roles = _context.Roles.AsQueryable();
            var menus = _context.Menus.AsQueryable();

            ViewBag.Roles = roles;
            ViewBag.Menus = menus;

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }

            var menuRoles = _context.MenuRoles
                .Where(mr => mr.MenuId == menu.Id)
                .Select(mr => mr.RoleId)
                .ToList();

            var viewModel = _mapper.Map<MenuViewModel>(menu);

            viewModel.SelectedRoleIds = new List<string>();

            foreach (var role in ViewBag.Roles)
            {
                if (menuRoles.Contains(role.Id))
                {
                    viewModel.SelectedRoleIds.Add(role.Id);
                }
            }

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MenuViewModel menu)
        {
            if (id != menu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Menu menuCriado = await _context.Menus.FindAsync(id);

                if (menuCriado != null)
                {
                    menuCriado.CreatedAt = menuCriado.CreatedAt;
                    menuCriado.UpdatedAt = DateTime.Now;
                    menuCriado.Name = menu.Name;
                    menuCriado.Icon = menu.Icon;
                    menuCriado.Route = menu.Route;
                    menuCriado.ParentMenuId = menu.ParentMenuId;

                    var roles = _context.MenuRoles.Where(r => r.MenuId == menu.Id);

                    foreach (var item in roles)
                    {
                        _context.MenuRoles.Remove(item);
                    }

                    var RoleIds = Request.Form["chkRoles"];

                    foreach (var roleId in RoleIds)
                    {
                        MenuRole menuRole = new MenuRole();
                        menuRole.RoleId = roleId;
                        menuRole.MenuId = menu.Id;
                        _context.Add(menuRole);
                    }

                    _context.Update(menuCriado);


                    await _context.SaveChangesAsync();

                    BasicNotification("Menu salvo com sucesso!", NotificationType.Success);
                }
                else
                    return NotFound();


                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Métodos Privados
        private bool MenuExists(int id)
        {
            return _context.Menus.Any(e => e.Id == id);
        }
        #endregion
    }
}
