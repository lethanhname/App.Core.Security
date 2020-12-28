using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using App.Core.Contract.Security;
using App.Core.Security.Contract;
using App.CoreLib.EF.Data;
using App.CoreLib.EF.Data.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using IdentityServer4.AccessTokenValidation;
using App.Core.Security.Web.PolicyServices;
using Microsoft.AspNetCore.Identity;
using App.CoreLib.Extensions;
using App.Core.Web.AppControllers;

namespace App.Core.Security.Web.Areas.Security.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.HasActionPermission)]
    [Area(Constants.AreaName)]
    public class RoleApiController : ApiControllerBase  
    {
        public IRepository<AppRole> _roleRepository { get; private set; }
        public IRoleQuery _roleQuery { get; private set; }
        private readonly RoleManager<AppRole> _roleManager;
        
        public RoleApiController(IRoleQuery roleQuery, IRepository<AppRole> roleRespository, RoleManager<AppRole> roleManager)
        {
            this._roleQuery = roleQuery;
            this._roleRepository = roleRespository;
            this._roleManager = roleManager;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery]QueryRequestBase request)
        {
            var gridData = await this._roleQuery.ReadDataAsync(request);
            return Ok(gridData);
        }

        [HttpPost("Create")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody]AppRole createOrEdit)
        {
            IdentityResult result = null;
            if (!ModelState.IsValid)
            {
                result = new IdentityResult();
                var errors = new List<IdentityError>();
                foreach(var error in ModelState)
                {
                    foreach(var message in error.Value.Errors)
                    {
                        errors.Add(new IdentityError{ Code = error.Key, Description = message.ErrorMessage});
                    }
                }
                result = IdentityResult.Failed(errors.ToArray());
                return new JsonResult(result);
            }
            
            if (createOrEdit.RowVersion == 0)
            {
                result = await _roleManager.CreateAsync(createOrEdit);
            }
            else
            {
                var role = await _roleManager.FindByNameAsync(createOrEdit.Name);
                role.Product = createOrEdit.Product;
                result = await _roleManager.UpdateAsync(role);
            }
            return new JsonResult(result);
        }
    }
}