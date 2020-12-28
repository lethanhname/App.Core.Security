


using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using App.CoreLib.EF.Data;

namespace App.Core.Security.Contract

{
    public interface IRolePermissionRepository : IRepository<RolePermission>
    {
        Task<IEnumerable<RolePermission>> FilteredByRoleAsync(string roleId);
        Task<bool> HashPermissionAsync(string userName, string area, string controller, string action);
    }
}