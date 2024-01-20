using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Salao.Repository.Models;
using Salao.WebApplication.Extensions;
using Salao.Repository.Models.Account;
using Salao.Repository.Models.Account.Views;
using System.Security.Claims;

namespace Salao.WebApplication.Controllers
{
    public class AccountController : BaseController
    {
        #region Propriedades
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        #endregion

        #region Contrutores
        public AccountController(
            ILogger<AccountController> logger,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
        }
        #endregion

        #region Metodos Publicos

        [HttpGet]
        //[Authorize("Permissions.Accounts.View")]
        public ActionResult Index()
        {
            var users = _userManager.Users.AsQueryable();
            return View(users.ToList());
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.IsPersistent, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.Name, user.UserName)
                        };

                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles.Count > 0)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, roles[0]));
                        }

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                        var passwordHasher = new PasswordHasher<ApplicationUser>();
                        bool isGenericPassword = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, user.GenericPassword) == PasswordVerificationResult.Success;

                        if (isGenericPassword)
                        {
                            return RedirectToAction("ChangePassword", "Account");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Tentativa de Login inválida.");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Tentativa de Login inválida.");
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(AccountController.Index), "Index");
        }

        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                base.BasicNotification("Email não encontrado!", NotificationType.Info);

                return RedirectToAction("ForgotPassword");
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = Url.Action("ResetPassword", "Account", new { email, token = resetToken }, Request.Scheme);

            //if (callbackUrl != null)
            //    await _emailService.SendEmailAsync(email, "Redefinição de Senha", "", callbackUrl);

            return RedirectToAction("ForgotPasswordConfirmation");
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            ClaimsPrincipal user = HttpContext.User;

            if (User == null)
            {
                return NotFound();
            }

            ViewBag.PrimeiroAcesso = false;

            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                var usuario = await _userManager.FindByEmailAsync(user.FindFirstValue(ClaimTypes.Email));
                if (usuario != null)
                {
                    var passwordHasher = new PasswordHasher<ApplicationUser>();

                    var isGenericPassword = passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, usuario.GenericPassword) == PasswordVerificationResult.Success;

                    if (isGenericPassword)
                    {
                        ViewBag.PrimeiroAcesso = true;
                    }
                }
            }

            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                    if (changePasswordResult.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        foreach (var error in changePasswordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    return NotFound();
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult ResetPasswordError()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            // Verifique se o email e o token são válidos
            var user = await _userManager.FindByEmailAsync(email);
            var isValidToken = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token);

            // Você pode passar o email e o token para a view ResetPassword para serem usados no formulário
            return View(new ResetPasswordViewModel { Email = email, Token = token });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    return RedirectToAction("ResetPasswordError");
                }

                var resetPasswordResult = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

                if (resetPasswordResult.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation");
                }

                foreach (var error in resetPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        #endregion

        #region Metodos Privados
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        #endregion
    }
}
