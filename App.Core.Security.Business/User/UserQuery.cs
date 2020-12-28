


using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using App.Core.Security.Contract;
using App.CoreLib.EF;
using App.CoreLib.EF.Data;
using App.CoreLib.EF.Data.Identity;
using App.CoreLib.EF.Data.Repositories;

namespace App.Core.Security.Business
{

    public class UserQuery : QueryBase<AppUser>, IUserQuery
    {
        public UserQuery(IStorage context) : base(context)
        {

        }
        protected override void DefaultSort(IQueryRequest request)
        {
                request.OrderColumn = "UserName";
                request.SortDirection = "asc";
        }

        protected override IQueryable<AppUser> Define(IQueryRequest request)
        {
            var dbset = storageContext.Set<AppUser>().AsNoTracking();
            return dbset.Where(r => r.Email.Contains(request.SearchValue));
        }
    }
}