using System;
using System.Collections.Generic;
using System.Text;
using App.Core.Contract.Security;

namespace App.Core.Security.Web.Common
{
    public static class Helpers
    {
        public static string GetPermissionCode(string controller, string action)
        {
            return SecurityHelpers.GetPermissionCode(Constants.AreaName, controller, action);
        }
        public static string GetPermissionCode(string controller)
        {
            return SecurityHelpers.GetPermissionCode(Constants.AreaName, controller, "Index");
        }
    }
}
