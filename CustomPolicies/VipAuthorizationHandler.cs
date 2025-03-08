using BankAccountManager.Enums;
using Microsoft.AspNetCore.Authorization;

namespace BankAccountManager.CustomPolicies;

public class VipAuthorizationHandler : AuthorizationHandler<VipAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        VipAuthorizationRequirement requirement
    )
    {
        if (
            context.User.IsInRole(AppUserRoles.VipUser.ToString())
            && requirement.AccountType == AccountTypes.Savings
        )
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}
