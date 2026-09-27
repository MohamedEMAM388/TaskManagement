using Infrastructure.Persistence;
using Infrastructure.Persistence.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.BackgroundJobs;

public class RefreshTokenCleanupJob (IdentityAppDbContext context)
{
    public async Task DeleteExpiredTokensAsync()
    {
        var cutOff = DateTime.UtcNow;
        await context.RefreshTokens.Where(t => t.ExpiresOn <= cutOff
                                               || !t.IsActive ).ExecuteDeleteAsync();
    }
}