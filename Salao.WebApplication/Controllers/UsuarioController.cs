
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Salao.WebApplication.Controllers;
using Salao.WebApplication.Extensions;
using Salao.Repository.Models;
using Salao.Repository.Models.Account;
using Salao.Repository.Models.Account.Views;
using System.Text;
using Salao.Repository.Models.Usuario.Views;
using AutoMapper;

namespace Reanvam.App.Controllers
{
    public class UsuarioController : BaseController
    {
        #region Propriedades
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        #endregion

        #region Contrutores
        public UsuarioController(ILogger<AccountController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }
        #endregion

        #region Metodos Públicos
        [HttpGet]
        //[Authorize("Permissions.Accounts.View")]
        public async Task<ActionResult> Index()
        {
            var users = _context.Users.ToList();

            List<UsuarioViewModel> lstModel = new List<UsuarioViewModel>();

            foreach (var item in users)
            {
                var viewModel = await ObterUsuarioViewModel(item);
                lstModel.Add(viewModel);
            }

            ViewBag.Roles = await _context.Roles.ToListAsync();

            return View(lstModel);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Roles = _context.Roles.ToList();
            return View();
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            var usuario = _context.Users.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<UsuarioViewModel>(usuario));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            ViewBag.Roles = _context.Roles.ToList();

            var usario = await _context.Users.FindAsync(id);

            if (usario != null)
            {
                var viewModel = await ObterUsuarioViewModel(usario);

                return View(viewModel);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var usuario = await _context.Users.FindAsync(id);

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterViewModel model, string roleId)
        {
            ApplicationUser applicationUser = new ApplicationUser();

            if (ModelState.IsValid)
            {
                applicationUser = new ApplicationUser { UserName = model.Email, FullName = model.FullName, Email = model.Email };

                if (applicationUser != null)
                {
                    var jaExiste = await _context.Users.Where(x => x.Email == applicationUser.Email).AnyAsync();

                    if (jaExiste)
                    {
                        BasicNotification("Já existe um usuário cadastrado com este e-mail!", NotificationType.Error);
                        return RedirectToAction(nameof(Index));
                    }

                    string generatedPassword = GenerateRandomPassword();

                    var result = await _userManager.CreateAsync(applicationUser, generatedPassword);

                    if (result.Succeeded)
                    {
                        applicationUser.GenericPassword = generatedPassword;
                        await _userManager.UpdateAsync(applicationUser);

                        var userRole = new IdentityUserRole<string> { UserId = applicationUser.Id, RoleId = roleId };
                        var resultado = await _context.UserRoles.AddAsync(userRole);

                        await SendPasswordEmail(applicationUser.Email, generatedPassword);

                        BasicNotification("Usuário cadastrado com sucesso! Um email foi enviado com a senha para o usuário cadastrado.", NotificationType.Success);

                        _context.SaveChanges();

                        return RedirectToAction(nameof(Index));
                    }
                    AddErrors(result);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private string GenerateRandomPassword()
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            Random random = new Random();
            StringBuilder password = new StringBuilder();

            for (int i = 0; i < 8; i++)
            {
                password.Append(validChars[random.Next(validChars.Length)]);
            }

            return password.ToString();
        }

        private async Task SendPasswordEmail(string email, string password)
        {
            var callbackUrl = Url.Action("Login", "Account", null, protocol: HttpContext.Request.Scheme);

            if (callbackUrl != null)
            {
                //await _emailService.SendPasswordEmail(email, password, callbackUrl);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser user, string roleId, Guid empresaId)
        {
            if (!await VerificarPerfilAnfavea(roleId, empresaId))
            {
                return RedirectToAction(nameof(Edit));
            }

            var usuario = await _context.Users.FindAsync(user.Id);

            if (usuario == null)
                return NotFound();

            usuario.Email = user.Email;
            usuario.UserName = user.Email;
            usuario.FullName = user.FullName;
            usuario.NormalizedUserName = user.NormalizedUserName;
            usuario.PhoneNumber = user.PhoneNumber;

            await _userManager.UpdateAsync(usuario);

            var roleExistente = await _context.UserRoles.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();

            if (roleExistente != null)
            {
                _context.Remove(roleExistente);
            }

            var userRole = new IdentityUserRole<string> { UserId = user.Id, RoleId = roleId };
            var resultado = await _context.UserRoles.AddAsync(userRole);

            _context.SaveChanges();

            BasicNotification("Usuário alterado com sucesso!", NotificationType.Success);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUsuario(string id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();
            else
                _context.Users.Remove(user);

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> SalvarRole(string Nome)
        {
            IdentityRole role = new IdentityRole();

            role.Name = Nome;

            var result = await _context.Roles.AddAsync(role);

            _context.SaveChanges();

            ViewBag.Roles = _context.Roles.ToList();

            return RedirectToAction(nameof(Create));
        }

        public async Task<IActionResult> ExcluirRole(string id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
                return NotFound();

            _context.Remove(role);

            _context.SaveChanges();


            return RedirectToAction(nameof(Create));
        }
        #endregion

        #region Metodos privados
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        private ApplicationUser GetUser(string UserName, string Email)
        {
            var user = new ApplicationUser
            {
                UserName = UserName,
                Email = Email
            };

            return user;
        }

        private async Task<UsuarioViewModel> ObterUsuarioViewModel(ApplicationUser user)
        {
            UsuarioViewModel viewModel = new UsuarioViewModel();

            viewModel.Id = user.Id;
            viewModel.Email = user.Email;
            viewModel.UserName = user.UserName;
            viewModel.FullName = user.FullName;

            var roles = _context.UserRoles.Where(x => x.UserId == user.Id);

            if (roles != null)
                viewModel.Roles = _context.Roles.Where(x => x.Id == roles.First().RoleId).Select(x => x.Name).AsEnumerable();

            return viewModel;
        }

        private async Task<bool> VerificarPerfilAnfavea(string roleId, Guid empresaId)
        {
            var role = await _context.Roles.Where(x => x.Name.ToLower().Contains("anfavea") || x.Name.ToLower().Contains("admin")).FirstOrDefaultAsync();

            if (role != null && roleId != role.Id && empresaId == Guid.Empty)
            {
                BasicNotification("Selecione a empresa!", NotificationType.Error);
                return false;
            }

            return true;
        }

        #endregion
    }
}
