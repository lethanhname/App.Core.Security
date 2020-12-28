using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using App.Core.Contract.Security;
using App.Core.Security.Contract;
using App.CoreLib.EF;
using App.CoreLib.EF.Data.Identity;
using App.CoreLib.EF.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace App.Core.Security.Business
{
    public class RolePermissionRepository : Repository<RolePermission>, IRolePermissionRepository
    {
        private UserManager<AppUser> _userManager;
        private RoleManager<AppRole> _roleManager;
        public RolePermissionRepository(IStorage context, UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager) : base(context)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        public async Task<IEnumerable<RolePermission>> FilteredByRoleAsync(string roleId)
        {
            return await this.DbSet.AsNoTracking().Where(rp => rp.RoleId == roleId).ToListAsync();
        }

        public async Task<bool> HashPermissionAsync(string userName, string area, string controller, string action)
        {
            if (!string.IsNullOrWhiteSpace(userName))
            {
                var user = await _userManager.FindByNameAsync(userName);
                var roles = await this._userManager.GetRolesAsync(user);

                var permissionCode = SecurityHelpers.GetPermissionCode(area, controller, action);
                var controllerPermissionCode = SecurityHelpers.GetPermissionCode(area, controller);
                foreach (var role in roles)
                {
                    var roleId = await this._roleManager.FindByNameAsync(role);
                    var permission = this.Find(roleId.Id, permissionCode);
                    if (permission != null)
                    {
                        return true;
                    }
                    var controllerPermission = this.Find(roleId.Id, controllerPermissionCode);
                    if (controllerPermission != null)
                    {
                        return true;
                    }
                    
                }
            }

            return false;
        }
    }
}