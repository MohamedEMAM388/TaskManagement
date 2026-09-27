using Hangfire.Dashboard;
using Microsoft.Extensions.DependencyInjection;

namespace API.HangFireAuthorization;

public class HangfireAdminAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        var env = httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();

        if (env.IsDevelopment())
            return true; 

        return httpContext.User.Identity?.IsAuthenticated == true
               && httpContext.User.IsInRole("Admin");
    }
}