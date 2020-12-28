using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using App.Core.Security.Contract;
using App.CoreLib.EF.Data;
using App.CoreLib.EF.Data.Repositories;
using App.CoreLib.EF;

namespace App.Core.Security.Business
{

    public class RolePermissionQuery : QueryBase<RolePermission>, IRolePermissionQuery
    {
        public RolePermissionQuery(IStorage context):base(context)
        {

        }
        protected override void DefaultSort(IQueryRequest request)
        {
                request.OrderColumn = "PermissionName";
                request.SortDirection = "asc";
        }

        protected override IQueryable<RolePermission> Define(IQueryRequest request)
        {
            var tbRolePermission = storageContext.Set<RolePermission>().AsNoTracking();
            var tbPermission = storageContext.Set<Permission>().AsNoTracking();
            var query = tbRolePermission.GroupJoin(tbPermission, t => t.PermissionId, g => g.Code, (t, g) => new { t, g })
            .SelectMany(p => p.g.DefaultIfEmpty(), (p, g) => new RolePermission
            {
                PermissionId = p.t.PermissionId,
                RoleId = p.t.RoleId
            });

            var internalRequest = request as RolePermissionQueryRequest;
            if (internalRequest != null)
            {
                if (string.IsNullOrWhiteSpace(internalRequest.RoleId))
                    query = query.Where(r => r.RoleId == internalRequest.RoleId);
            }
            query = query.Where(r => r.PermissionId.Contains(request.SearchValue));
            return query;
        }
    }
}