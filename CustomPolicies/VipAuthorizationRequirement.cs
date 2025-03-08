using System;
using BankAccountManager.Enums;
using Microsoft.AspNetCore.Authorization;

namespace BankAccountManager.CustomPolicies;

public class VipAuthorizationRequirement : IAuthorizationRequirement
{
    public AccountTypes AccountType { get; set; }

    public VipAuthorizationRequirement(AccountTypes accountType)
    {
        AccountType = accountType;
    }
}
