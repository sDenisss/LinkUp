using LinkUp.Domain;
using MediatR;

namespace LinkUp.Application;

public record AuthCommand(
    Email Email,
    string Password
) : IRequest<string>;