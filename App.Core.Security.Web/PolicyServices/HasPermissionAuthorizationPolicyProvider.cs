using Microsoft.AspNetCore.Authorization;
using App.Core.Contract.Security;
using App.Core.Web;

namespace App.Core.Security.Web.PolicyServices
{
    public class HasPermissionAuthorizationPolicyProvider : IAppAuthorizationPolicyProvider
    {
        public string Name => Policies.HasActionPermission;

        public AuthorizationPolicy GetAuthorizationPolicy()
        {
            AuthorizationPolicyBuilder authorizationPolicyBuilder = new AuthorizationPolicyBuilder();
            authorizationPolicyBuilder.Requirements.Add(new AppPolicyRequirement());

            return authorizationPolicyBuilder.Build();
        }

    }
}