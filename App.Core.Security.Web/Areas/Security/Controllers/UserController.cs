using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using App.Core.Contract.Security;
using App.Core.Security.Web.ViewModels.User;
using App.Core.Security.Web.ViewModels.UserRole;
using App.Core.Web.AppControllers;
using App.CoreLib.EF.Data;
using App.CoreLib.EF.Data.Entity;
using App.CoreLib.EF.Data.Identity;

namespace App.Core.Security.Web.Controllers
{
    [Authorize(Policy = Policies.HasActionPermission)]
    [Area(Constants.AreaName)]
    public class UserController : MaintenanceBase<AppUser>
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IRepository<AppRole> _roleRepository;


        protected override string BackToController => "UserInquiry";

        protected override string AddAction => "Index";

        public UserController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager, IEmailService emailService, IRepository<AppUser> repository,
            IRepository<AppRole> roleStore) :base(repository)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _emailService = emailService;
            _roleRepository = roleStore;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string userName)
        {
            var user = string.IsNullOrWhiteSpace(userName) ? null : _userManager.FindByNameAsync(userName).Result;
            var response = Define(user);
            await SetViewModelAsync(response, user);
            return this.View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Index(CreateUserViewModel model)
        {           
            if (this.ModelState.IsValid)
            {
                IdentityResult result = null;
                
                if (model.RowVersion == 0)
                {
                    var user = new AppUser
                    {
                        UserName = model.UserName,
                        GivenName = model.FirstName,
                        FamilyName = model.LastName,
                        Email = model.Email,
                    };
                    result = await _userManager.CreateAsync(user, "Undefined@1");
                    //if (result.Succeeded)
                    //{
                    //    //generation of the email token
                    //    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    //    var link = Url.Action("VerifyEmail", "Account", new { userId = user.Id, code }, Request.Scheme, Request.Host.ToString());

                    //    await _emailService.SendAsync(user.Email, "email verify", $"<a href=\"{link}\">Verify Email</a>", true);
                    //}
                }
                else
                {
                    var user = await _userManager.FindByNameAsync(model.UserName);
                    user.GivenName = model.FirstName;
                    user.FamilyName = model.LastName;
                    user.Email = model.Email;
                    result = await _userManager.UpdateAsync(user);
                }
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.UserName);
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    foreach (var role in model.UserRoles)
                    {
                        if(!role.IsAssigned && currentRoles.Contains(role.Name))
                        {
                            await _userManager.RemoveFromRoleAsync(user, role.Name);
                            currentRoles.Remove(role.Name);
                        }
                    }
                    var newRoles = model.UserRoles.Where(i => i.IsAssigned).Select(i => i.Name).ToList();
                    newRoles.RemoveAll(i => currentRoles.Contains(i));
                    var roleResult = await _userManager.AddToRolesAsync(user, newRoles);
                    if(!roleResult.Succeeded)
                    {
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return RedirectToAction("Index", "UserInquiry", new { Area = "Security" });
        }

        private async Task SetViewModelAsync(MaintenanceDefinition response, AppUser user)
        {
            var model = new CreateUserViewModel();
            if (user != null)
            {
                model = new CreateUserViewModel
                {
                    RowVersion = user.RowVersion,

                    UserName = user.UserName,
                    FirstName = user.GivenName,
                    LastName = user.FamilyName,

                    Email = user.Email
                };
            }            
            model.UserRoles = await GetUserRolesAsync(user);

            response.Data = model;
        }

        private async Task<List<UserRoleViewModel>> GetUserRolesAsync(AppUser user)
        {
            var userRoleModel = new List<UserRoleViewModel>();
            IList<string> userRoles = new List<string>();
            if(user != null) userRoles = await _userManager.GetRolesAsync(user);
            var roles = _roleRepository.Query().ToList();
            foreach (var role in roles)
            {
                userRoleModel.Add(new UserRoleViewModel
                {
                    Name = role.Name,
                    IsAssigned = userRoles.Contains(role.Name)
                });
            }
            return userRoleModel;
        }

        protected override string GetFormTitle(IEntity entity)
        {
            var model = entity as AppUser;
            return model != null ? string.Format("{0} {1}({2})", model.GivenName, model.FamilyName, model.UserName) : "Add New user" ;
        }
    }
}