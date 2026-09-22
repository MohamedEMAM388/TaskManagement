using Application.Common.ResultPattern;
using Application.Features.Authentication.Commands.DTOs;
using MediatR;

namespace Application.Features.Authentication.Commands.Register;

public sealed record RegisterCommand(RegisterDto RegisterDto) : IRequest<Result<UserDto>>;
