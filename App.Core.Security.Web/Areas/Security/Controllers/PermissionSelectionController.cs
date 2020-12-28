using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using App.Core.Web.AppControllers;
using App.CoreLib.EF.Data;
using App.Core.Security.Contract;
using App.CoreLib.EF.Data.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using App.CoreLib.EF.Data.Entity;
using Microsoft.AspNetCore.Authorization;
using App.Core.Contract.Security;

namespace App.Core.Security.Web.Controllers
{
    [Authorize(Policy = Policies.HasActionPermission)]
    [Area(Constants.AreaName)]
    public class PermissionSelectionController : InquiryBase<QueryRequestBase, Permission, IPermissionQuery>
    {
        private readonly RoleManager<AppRole> roleService;

        protected override List<FieldDefinition> FieldDefinitions => throw new System.NotImplementedException();

        protected override List<string> KeyFields => throw new System.NotImplementedException();

        public PermissionSelectionController(RoleManager<AppRole> roleManager, IPermissionQuery permissionQuery) : base(permissionQuery)
        {
            roleService = roleManager;
        }
        [HttpGet]
        public IActionResult Index(string name)
        {
            var response = Define(roleService.FindByNameAsync(name).Result);
            return this.View(response);
        }

        [HttpPost]
        public virtual async Task<ActionResult> AddSelected([FromBody]JObject body)
        {
            dynamic entity = body;
            var roleId = entity.RoleId;
            var role = await roleService.FindByIdAsync(roleId);
            foreach (var item in entity.SelectedItems)
            {
                await roleService.AddClaimAsync(role, new Claim(AppClaimTypes.Permission, item.Code));
            }
            return Json(new { redirect = Url.Action("Index", "RolePermission", new { name = role.Name}) });
        }

        //protected ControllerResponseBase GetResponse(AppRole entity)
        //{
        //    return new ControllerResponseBase{
        //        Title = "Permissions",
        //        ModelName = "Permission",
        //        ViewName = "ItemView",
        //        GetUrl = Url.Action("Get", "PermissionSelection"), 
        //        ListUrl = Url.Action("Index", "RolePermission", new { id = entity.Id}),
        //        Data = entity
        //    };
        //}

        protected override string GetFormTitle(IEntity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
