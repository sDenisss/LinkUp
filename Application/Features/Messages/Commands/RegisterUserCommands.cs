using MediatR;

namespace LinkUp.Application;
public record RegisterUserCommand(
    string Username,
    string Email,
    string Password,
    string UniqueName) : IRequest<Guid>;
