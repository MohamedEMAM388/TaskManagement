using System.Security.Claims;
using Domain.Common;
using Microsoft.AspNetCore.Authorization;

namespace Application.Common.Authorization;

public class ResourceOwnerAuthorizationHandler : AuthorizationHandler<ResourceOwnerRequirement , IHasOwner>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, // have all info about the current request like user and his claims
        ResourceOwnerRequirement requirement,
        IHasOwner resource)
    {
        // role-based check
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        
        // claim-based check
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        // compare current-user and resource-owner
        if (userId is not null && userId == resource.CreatedByUserId) 
              context.Succeed(requirement);
        
        // if fail by default 
        return Task.CompletedTask;
    }
}