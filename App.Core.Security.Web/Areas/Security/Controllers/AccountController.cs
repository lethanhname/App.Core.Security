using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;
using System.Threading.Tasks;
using App.Core.Security.Web.ViewModels.Account;
using App.Core.Web.AppControllers;
using App.CoreLib;
using App.CoreLib.EF.Data.Identity;
using System.Security.Claims;
using IdentityServer4.Services;

namespace App.Core.Security.Web.Controllers
{
    //[Authorize(Policy = Policies.HasActionPermission)]
    [Area(Constants.AreaName)]
    public class AccountController: AppControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;
        IIdentityServerInteractionService _interaction;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IEmailService emailService,
            IIdentityServerInteractionService _interaction)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            this._interaction = _interaction;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ChangePassword()
        {
            return this.View(GetResponse(null));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(Globals.CurrentUser);
                if (user != null)
                {
                    string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordChangeResult = await _userManager.ResetPasswordAsync(user, resetToken, model.Password);

                    if (passwordChangeResult.Succeeded)
                    {
                        return this.RedirectToAction("Index", "Home", new { area = "" });
                    }
                    else
                    {
                        return this.View(GetResponse(model));
                    }
                }
            }

            return this.RedirectToAction("Index", "Home", new { area = ""});
        }
        protected MaintenanceDefinition GetResponse(ChangePasswordViewModel entity)
        {
            var response = new MaintenanceDefinition();

            response.Title = "Change password";
            response.Data = entity == null ? new ChangePasswordViewModel() : entity;

            response.ViewName = "ItemView";

            return response;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult PasswordSent()
        {
            return this.View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignIn(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return this.View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn(SignInViewModel signIn, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (this.ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(signIn.Email);

                if (user != null)
                {
                    //sign in
                    var signInResult = await _signInManager.PasswordSignInAsync(user, signIn.Password, signIn.RememberMe, false);

                    if (signInResult.Succeeded)
                    {
                        await _userManager.AddClaimAsync(user, new Claim("givenname", user.GivenName));
                        var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
                        await _signInManager.RefreshSignInAsync(user);
                    
                        return RedirectToLocal(returnUrl);
                    }
                }

            }
            ModelState.AddModelError("", "Invalid login attempt");
            return this.CreateRedirectToSelfResult();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SignOut(string returnUrl = null)
        {
            var logoutId = this.Request.Query["logoutId"].ToString();
            await _signInManager.SignOutAsync();
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else if (!string.IsNullOrEmpty(logoutId))
            {
                var logoutContext = await this._interaction.GetLogoutContextAsync(logoutId);
                returnUrl = logoutContext.PostLogoutRedirectUri;

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return this.Redirect(returnUrl);
                }
            }
            return this.RedirectToLocal(returnUrl);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return this.View();
        }

        public async Task<IActionResult> VerifyEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return BadRequest();

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                return View();
            }

            return BadRequest();
        }

        public IActionResult EmailVerification() => View();

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
