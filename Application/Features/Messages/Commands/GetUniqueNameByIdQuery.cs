using MediatR;

namespace LinkUp.Application;

public record GetUniqueNameByIdQuery(
    Guid UserId
) : IRequest<UserDto>;