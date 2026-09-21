using Application.Features.Authentication.Commands.DTOs;
using Application.Common.ResultPattern;
using MediatR;

namespace Application.Features.Authentication.Commands.Login;

public sealed record LoginCommand(LoginDto LoginDto) : IRequest<Result<UserDto>>;
