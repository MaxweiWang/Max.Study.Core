using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Core.Utility.Authentications
{
    public class RoleAuthorizationRequirement : AuthorizationHandler<RoleAuthorizationRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleAuthorizationRequirement requirement)
        {
            throw new NotImplementedException();
        }
    }
}
