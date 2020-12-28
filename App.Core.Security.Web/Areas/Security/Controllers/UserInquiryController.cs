using App.Core.Web.AppControllers;
using App.CoreLib.EF.Data;
using App.Core.Security.Contract;
using App.CoreLib.EF.Data.Identity;
using System.Collections.Generic;
using App.CoreLib.EF.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using App.Core.Contract.Security;

namespace App.Core.Security.Web.Controllers
{
    [Authorize(Policy = Policies.HasActionPermission)]
    [Area(Constants.AreaName)]
    public class UserInquiryController : InquiryBase<QueryRequestBase, AppUser, IUserQuery>
    {
        public UserInquiryController(IUserQuery storage) : base(storage)
        {
        }
        [HttpGet]
        public virtual IActionResult Index()
        {
            return InquiryView(Define(null));
        }
        protected override List<FieldDefinition> FieldDefinitions => new List<FieldDefinition>{
                new FieldDefinition{title = "UserName", data = "userName"},
                new FieldDefinition{title = "Email", data = "email"},
                new FieldDefinition{title = "PhoneNumber", data = "phoneNumber"},
            };

        protected override List<string> KeyFields => new List<string> { "userName" };


        protected override string MaintenanceController => "User";
        protected override string MaintenanceEditAction => "Index";



        protected override string GetFormTitle(IEntity entity)
        {
            return "Users";
        }
    }
}
