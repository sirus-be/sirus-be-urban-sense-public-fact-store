using FactStore.Models.Authorization;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FactStore.Api.Handlers
{
    public class RoleAuthorizationHandler : AuthorizationHandler<ValidRoleRolesRequirement, IEnumerable<string>>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       ValidRoleRolesRequirement requirement,
                                                       IEnumerable<string> roles)
        {
            var userRoles = context.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            var validatedRoles = roles.Where(fR => userRoles.Any(uR => uR == fR));
            if (userRoles.Contains(Roles.SuperAdmin) || validatedRoles.Any())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class ValidRoleRolesRequirement : IAuthorizationRequirement { }
}
