using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Salao.Repository.Models.Account;
using Salao.Repository.Models;
using Salao.WebApplication.Extensions;
using Salao.WebApplication.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace Salao.WebApplication.Controllers
{
    public class HomeController : BaseController
    {
        #region Propriedades
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        #endregion

        #region Construtores
        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        #endregion

        #region Métodos Públicos
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        #endregion

        #region Métodos Privados

        #endregion
    }
}