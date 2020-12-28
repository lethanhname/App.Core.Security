using System.Collections.Generic;
using App.CoreLib.EF.Data;

namespace App.Core.Security.Contract
{
    public interface IRolePermissionQuery : IQueryBase<RolePermission>
    {

    }
    public class RolePermissionQueryRequest : QueryRequestBase
    {
        public string RoleId { get; set; }
    }
}