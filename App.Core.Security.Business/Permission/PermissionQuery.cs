


using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using App.Core.Security.Contract;
using App.CoreLib.EF;
using App.CoreLib.EF.Data;
using App.CoreLib.EF.Data.Repositories;

namespace App.Core.Security.Business
{

    public class PermissionQuery : QueryBase<Permission>, IPermissionQuery
    {
        public PermissionQuery(IStorage context):base(context)
        {

        }
        protected override void DefaultSort(IQueryRequest request)
        {
                request.OrderColumn = "Name";
                request.SortDirection = "asc";
        }

        protected override IQueryable<Permission> Define(IQueryRequest request)
        {
            var dbset = storageContext.Set<Permission>().AsNoTracking();

            return dbset.Where(r => r.Name.Contains(request.SearchValue));
        }
    }
}