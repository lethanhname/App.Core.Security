using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using App.Core.Contract.Security;
using App.Core.Web.AppControllers;
using App.CoreLib.EF.Data;
using App.CoreLib.EF.Data.Entity;
using App.CoreLib.EF.Data.Identity;
using App.Core.Security.Web.PolicyServices;

namespace App.Core.Security.Web.Controllers
{
    [Area(Constants.AreaName)]
    public class RoleController : MaintenanceBase<AppRole>
    {
        private readonly RoleManager<AppRole> _roleManager;
        public RoleController(IRepository<AppRole> storage, RoleManager<AppRole> roleManager) : base(storage)
        {
            _roleManager = roleManager;
        }

        protected override string BackToController => "RoleInquiry";

        protected override string AddAction => "Index";

        [HttpGet]
        public IActionResult Index(string name)
        {
            var role = string.IsNullOrWhiteSpace(name) ? null : _roleManager.FindByNameAsync(name).Result;
            var response = Define(role);
            return this.MaintenanceView(response);
        }
        
        [HttpPost]
        public IActionResult Index(AppRole model)
        {
            if (this.ModelState.IsValid)
            {
                CreateOrEditRole(model);
                if (!this.ModelState.IsValid)
                {
                    return this.MaintenanceView(Define(model));
                }
            }
            return this.RedirectToAction("Index", new { model.Name});
        }
        private void CreateOrEditRole(AppRole createOrEdit)
        {
            IdentityResult result = null;
            if (createOrEdit.RowVersion == 0)
            {
                result = _roleManager.CreateAsync(createOrEdit).Result;
            }
            else
            {
                var role = _roleManager.FindByNameAsync(createOrEdit.Name).Result;
                role.Product = createOrEdit.Product;
                result = _roleManager.UpdateAsync(role).Result;
            }
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }

        protected override string GetFormTitle(IEntity entity)
        {
            var role = entity as AppRole;
            return (role != null) ? string.Format("{0} ({1})", role.Name, role.Name) : "Add New Role";
        }

        protected override List<LinkedItem> GetLinkedControlers(IEntity entity)
        {
            var linkedItems = new List<LinkedItem>();
            var role = entity as AppRole;
            if (role != null)
            {
                GetRoleLinkedControllers(linkedItems, role, "Details", Url);
            }
            return linkedItems;
        }

        public static void GetRoleLinkedControllers(List<LinkedItem> linkedItems, AppRole role, string activeItemTitle, IUrlHelper helper)
        {
            linkedItems.Add(new LinkedItem { Title = "Details", Url = helper.Action("Index", "Role", new { name = role.Name }) });
            linkedItems.Add(new LinkedItem { Title = "Permissions", Url = helper.Action("Index", "RolePermission", new { name = role.Name }) });

            var activeItem = linkedItems.FirstOrDefault(i => i.Title == activeItemTitle);
            activeItem.IsActive = true;
        }
    }
}
