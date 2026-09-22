using Application.Common.ResultPattern;
using Application.Contracts;
using Application.Features.Authentication.Commands.Login;
using MediatR;

namespace Application.Features.Authentication.Commands.LogOut;

public class LogOutHandler(IIdentityService identityService) : IRequestHandler<LogOutCommand , Result>
{
    public async Task<Result> Handle(LogOutCommand request, CancellationToken cancellationToken)
    {
       return await identityService.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);
    }
}