using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Max.Core.Utility.Authentications
{

    /// <summary>
    /// Policy 的策略 或者是规则
    /// </summary>
    public class AdvancedRequirement : AuthorizationHandler<NameAuthorizationRequirement>, IAuthorizationRequirement
    { 
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, NameAuthorizationRequirement requirement)
        {
            // 这里可以把用户信息获取到以后通过数据库进行验证
            // 这里就可以做一个规则验证
            // 也可以通过配置文件来验证
            if (context.User != null && context.User.HasClaim(c => c.Type == ClaimTypes.Sid))
            {
                string sid = context.User.FindFirst(c => c.Type == ClaimTypes.Sid).Value;
                if (!sid.Equals(requirement.RequiredName))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
