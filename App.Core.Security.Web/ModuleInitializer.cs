using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using App.Core.Security.Contract;
using App.CoreLib.EF;
using App.CoreLib.Module;
using App.Core.Security.Business;
using App.CoreLib.EF.Data.Identity;
using App.CoreLib.EF.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using App.Core.Security.Web.PolicyServices;
using System.Linq;
using App.CoreLib;
using App.Core.Web;
using App.Core.Security.Web.Common;
namespace App.Core.Security.Web
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IPermissionQuery, PermissionQuery>();
            services.AddScoped<IRoleQuery, RoleQuery>();
            services.AddScoped<IRolePermissionQuery, RolePermissionQuery>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<IUserQuery, UserQuery>();
            services.AddScoped<IAuthorizationHandler, AppPolicyHandler>();
            services.AddAuthorization(options =>
            {
                foreach (IAppAuthorizationPolicyProvider authorizationPolicyProvider in ExtensionManager.GetInstances<IAppAuthorizationPolicyProvider>())
                    options.AddPolicy(authorizationPolicyProvider.Name, authorizationPolicyProvider.GetAuthorizationPolicy());
            });
        }

        public void Configure(IApplicationBuilder applicationBuilder, IWebHostEnvironment env)
        {

            ILogger logger = applicationBuilder.ApplicationServices.GetService<ILoggerFactory>().CreateLogger("Core.Security.Web");

            logger.LogInformation("seeding data to database");

            SeedData(applicationBuilder, logger);
        }

        private void SeedData(IApplicationBuilder applicationBuilder, ILogger logger)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                logger.LogInformation("seeding permissions to database");
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<AppRole>>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<AppUser>>();
                var permissionRepository = serviceScope.ServiceProvider.GetRequiredService<IRepository<Permission>>();
                if (roleManager.FindByNameAsync("admin").Result == null)
                {
                    using (var transaction = permissionRepository.BeginTransaction())
                    {
                        bool success = true;
                        permissionRepository.Add(new Permission { Product = "Security", FunctionName = "List Roles", Code = Helpers.GetPermissionCode("RoleInquiry"), Name = "View Role", RowVersion = 1 });
                        permissionRepository.Add(new Permission { Product = "Security", FunctionName = "Maintenance Role", Code = Helpers.GetPermissionCode("Role"), Name = "Maintenance Role", RowVersion = 1 });
                        permissionRepository.Add(new Permission { Product = "Security", FunctionName = "Maintenance Role Permission", Code = Helpers.GetPermissionCode("RolePermission"), Name = "View Role permissions", RowVersion = 1 });

                        permissionRepository.Add(new Permission { Product = "Security", FunctionName = "List Users", Code = Helpers.GetPermissionCode("UserInquiry"), Name = "View User", RowVersion = 1 });
                        permissionRepository.Add(new Permission { Product = "Security", FunctionName = "Maintenance User", Code = Helpers.GetPermissionCode("User"), Name = "View User", RowVersion = 1 });
                        var permissionResult = permissionRepository.SaveChangesAsync().Result;
                        if (!permissionResult.Succeeded) success = false;
                        logger.LogInformation(permissionResult.Errors.ToString());

                        var adminRole = new AppRole { Name = "admin" };
                        var identityResult = roleManager.CreateAsync(adminRole).Result;
                        if (!identityResult.Succeeded) success = false;
                        logger.LogInformation(identityResult.Errors.ToString());

                        var permissions = permissionRepository.Query().ToList();
                        var rolePermissionRepository = serviceScope.ServiceProvider.GetRequiredService<IRolePermissionRepository>();
                        foreach (var permission in permissions)
                        {
                            rolePermissionRepository.Add(new RolePermission { RoleId = adminRole.Id, PermissionId = permission.Code });
                        }
                        var rolePermissionResult = rolePermissionRepository.SaveChangesAsync().Result;
                        if (!rolePermissionResult.Succeeded) success = false;
                        logger.LogInformation(rolePermissionResult.Errors.ToString());

                        var user = new AppUser
                        {
                            UserName = "undefined",
                            Email = "undefined@undefined.com",
                            GivenName = "undefined",
                            FamilyName = "undefined"
                        };
                        identityResult = userManager.CreateAsync(user, "Undefined@1").Result;
                        if (!identityResult.Succeeded) success = false;
                        logger.LogInformation(identityResult.Errors.ToString());

                        identityResult = userManager.AddToRoleAsync(user, adminRole.Name).Result;
                        if (!identityResult.Succeeded) success = false;
                        logger.LogInformation(identityResult.Errors.ToString());

                        if (success)
                            transaction.Commit();
                    }
                }
            }
        }
    }
}
