using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using App.Core.Contract.Security;
using App.Core.Security.Contract;
using App.CoreLib.EF.Data;
using App.CoreLib.EF.Data.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using IdentityServer4.AccessTokenValidation;
using IdentityModel;
using System.Security.Claims;
using App.Core.Security.Web.Areas.Security.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace App.Core.Security.Web.Areas.Security.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Policy = Policies.HasActionPermission)]
    [Area(Constants.AreaName)]
    public class IdentityController : ControllerBase  
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger _logger;

        public IdentityController(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            SignInManager<AppUser> signInManager,
            ILogger<IdentityController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;

            _logger = logger;
        }

        // GET api/identity/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var role = await _roleManager.FindByNameAsync("user");
            var users = await _userManager.GetUsersInRoleAsync(role.Name);

            return new JsonResult(users);
        }

        // POST: api/identity/Create
        [HttpPost("Create")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody]CreateViewModel model)
        {
            var user = new AppUser
            {
                GivenName = model.givenName,
                FamilyName = model.familyName,
                AccessFailedCount = 0,
                Email = model.username,
                EmailConfirmed = false,
                LockoutEnabled = true,
                NormalizedEmail = model.username.ToUpper(),
                NormalizedUserName = model.username.ToUpper(),
                TwoFactorEnabled = false,
                UserName = model.username
            };

            var result = await _userManager.CreateAsync(user, model.password);

            if (result.Succeeded)
            {
                await addToRole(model.username, "admin");
                await addClaims(model.username);
            }

            return new JsonResult(result);
        }

        // POST: api/identity/Delete
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete([FromBody]string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            var result = await _userManager.DeleteAsync(user);

            return new JsonResult(result);
        }

        private async Task addToRole(string userName, string roleName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            await _userManager.AddToRoleAsync(user, roleName);
        }

        private async Task addClaims(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var claims = new List<Claim> {
                new Claim(type: JwtClaimTypes.GivenName, value: user.GivenName),
                new Claim(type: JwtClaimTypes.FamilyName, value: user.FamilyName),
            };
            await _userManager.AddClaimsAsync(user, claims);
        }
    }
}