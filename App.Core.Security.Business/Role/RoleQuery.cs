


using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using App.CoreLib.EF.Data.Repositories;
using App.Core.Security.Contract;
using App.CoreLib.EF.Data;
using App.CoreLib.EF;
using App.CoreLib.EF.Data.Identity;

namespace App.Core.Security.Business
{

    public class RoleQuery : QueryBase<AppRole>, IRoleQuery
    {
        public RoleQuery(IStorage context):base(context)
        {

        }
        protected override void DefaultSort(IQueryRequest request)
        {
                request.OrderColumn = "Name";
                request.SortDirection = "asc";
        }

        protected override IQueryable<AppRole> Define(IQueryRequest request)
        {
            var dbset = storageContext.Set<AppRole>().AsNoTracking();
            if(!string.IsNullOrWhiteSpace(request.SearchValue))
                return dbset.Where(r => r.Name.Contains(request.SearchValue));
            else
                return dbset;
        }
    }
}