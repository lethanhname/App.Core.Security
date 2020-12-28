using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using App.Core.Security.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace App.Core.Security.Web.PolicyServices
{
    public class AppPolicyRequirement: IAuthorizationRequirement
    {
    }
    public class AppPolicyHandler : AuthorizationHandler<AppPolicyRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IRolePermissionRepository _rolePermissionRepository;
        
        public AppPolicyHandler(IHttpContextAccessor httpContextAccessor, IRolePermissionRepository rolePermissionRepository)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            this._rolePermissionRepository = rolePermissionRepository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AppPolicyRequirement requirement)
        {
            var filterContext = context.Resource as AuthorizationFilterContext;
            var response = filterContext?.HttpContext.Response;

            if (!context.User.Identity.IsAuthenticated || string.IsNullOrEmpty(context.User.Identity.Name))
            {
                response?.OnStarting(async () =>
                {
                    filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                });

                context.Fail();
                return;
                // return Task.CompletedTask;
            }

            var hasPermission = false;
            var userName = context.User.Identity.Name;
            var routeData = _httpContextAccessor.HttpContext.GetRouteData();

            var areaName = routeData?.Values["area"]?.ToString();
            var area = string.IsNullOrWhiteSpace(areaName) ? string.Empty : areaName;

            var controllerName = routeData?.Values["controller"]?.ToString();
            var controller = string.IsNullOrWhiteSpace(controllerName) ? string.Empty : controllerName;

            var actionName = routeData?.Values["action"]?.ToString();
            var action = string.IsNullOrWhiteSpace(actionName) ? string.Empty : actionName;

            hasPermission = await _rolePermissionRepository.HashPermissionAsync(userName, area.ToString(), controller.ToString(), actionName.ToString());

            if (hasPermission)
            {
                // user belong to a role associated to the route.
                context.Succeed(requirement);
                return;
                // return Task.CompletedTask;
            }

            response?.OnStarting(async () =>
            {
                filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
            });

            context.Fail();
            // return Task.CompletedTask;
        }

    }
}