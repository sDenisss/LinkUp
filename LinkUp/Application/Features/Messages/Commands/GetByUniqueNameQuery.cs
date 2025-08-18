using MediatR;

namespace LinkUp.Application;

public record GetByUniqueNameQuery(
    string UniqueName
) : IRequest<UserDto>;