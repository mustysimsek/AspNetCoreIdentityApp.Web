﻿using System;
using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreIdentityApp.Web.Requirements
{
    public class ExchangeExpireRequirement : IAuthorizationRequirement
    {

    }

    public class ExchangeExpireRequirementHandler : AuthorizationHandler<ExchangeExpireRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExchangeExpireRequirement requirement)
        {

            if (!context.User.HasClaim(x => x.Type == "ExchangeExpireDate"))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var exchangeExpireDate = context.User.FindFirst("ExchangeExpireDate");

            if (DateTime.Now > Convert.ToDateTime(exchangeExpireDate.Value))
            {
                context.Fail();
                return Task.CompletedTask;
            }


            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}

