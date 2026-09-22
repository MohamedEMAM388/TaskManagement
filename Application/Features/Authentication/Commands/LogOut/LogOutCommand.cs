using Application.Common.ResultPattern;
using MediatR;

namespace Application.Features.Authentication.Commands.LogOut;

public sealed record LogOutCommand(string RefreshToken) : IRequest<Result>;