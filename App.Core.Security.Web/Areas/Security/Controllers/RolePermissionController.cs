using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using App.Core.Web.AppControllers;
using App.Core.Security.Contract;
using App.CoreLib.EF.Data.Identity;
using Microsoft.AspNetCore.Identity;
using App.CoreLib.EF.Data.Entity;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using App.Core.Contract.Security;

namespace App.Core.Security.Web.Controllers
{
    [Authorize(Policy = Policies.HasActionPermission)]
    [Area(Constants.AreaName)]
    public class RolePermissionController : InquiryBase<RolePermissionQueryRequest, RolePermission, IRolePermissionQuery>
    {
        private readonly RoleManager<AppRole> roleService;
        public RolePermissionController(IRolePermissionQuery storage, RoleManager<AppRole> roleManager) : base(storage)
        {
            roleService = roleManager;
        }

        protected override string BackToController => "RoleInquiry";
        protected override string BackToAction => "Index";

        protected override List<FieldDefinition> FieldDefinitions => new List<FieldDefinition>
                {
                    new FieldDefinition{title = "Code", data = "code"},
                    new FieldDefinition{title = "Name", data = "name"},
                    new FieldDefinition{title = "FunctionName", data = "functionName"}
                };

        protected override List<string> KeyFields => new List<string> { "code" };

        [HttpGet]
        public IActionResult Index(string name)
        {
            var response = Define(roleService.FindByNameAsync(name).Result);
            return InquiryView(response);
        }

        protected override void SetRequestData(RolePermissionQueryRequest request)
        {
            var data = HttpContext.Request.Form["id"].FirstOrDefault();
            request.RoleId = data;
        }

        protected override string GetFormTitle(IEntity entity)
        {
            var role = entity as AppRole;
            return role == null ? "Add New Role" : string.Format("{0} ({1})", role.Name, role.Name);
        }
        protected override List<LinkedItem> GetLinkedControlers(IEntity entity)
        {
            var linkedItems = new List<LinkedItem>();
            var role = entity as AppRole;
            if (role != null)
            {
                RoleController.GetRoleLinkedControllers(linkedItems, role, "Permissions", Url);
            }
            return linkedItems;
        }

    }
}
