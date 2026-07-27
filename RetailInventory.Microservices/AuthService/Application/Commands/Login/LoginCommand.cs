using MediatR;

namespace AuthService.Application.Commands.Login;

public record LoginCommand(string Username,string Password): IRequest<Result<string>>;