using MediatR;

namespace LinkUp.Application;

public record GetAllUsersInChatQuery(
    Guid ChatId
) : IRequest<IEnumerable<UserDto>>;