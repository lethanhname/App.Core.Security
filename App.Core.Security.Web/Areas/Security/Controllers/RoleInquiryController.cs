using Microsoft.AspNetCore.Mvc;
using App.Core.Web.AppControllers;
using App.CoreLib.EF.Data;
using App.Core.Security.Contract;
using App.CoreLib.EF.Data.Identity;
using System.Collections.Generic;
using App.CoreLib.EF.Data.Entity;
using Microsoft.AspNetCore.Authorization;
using App.Core.Contract.Security;
using App.Core.Security.Web.PolicyServices;

namespace App.Core.Security.Web.Controllers
{
    [Authorize(Policy = Policies.HasActionPermission)]
    [Area(Constants.AreaName)]
    public class RoleInquiryController : InquiryBase<QueryRequestBase, AppRole, IRoleQuery>
    {
        public RoleInquiryController(IRoleQuery storage) : base(storage)
        {
        }

        protected override List<FieldDefinition> FieldDefinitions => new List<FieldDefinition>
        {
            new FieldDefinition{title = "Name", data = "name"},
            new FieldDefinition{title = "Product", data = "product"}
        };
        protected override List<string> KeyFields => new List<string> { "name" };


        protected override string MaintenanceController => "Role";
        protected override string MaintenanceEditAction => "Index";


        protected override string DeleteAction => "";

        [HttpGet]
        public virtual IActionResult Index()
        {
            return InquiryView(Define(null));
        }

        protected override string GetFormTitle(IEntity entity)
        {
            return "Roles";
        }
    }
}
