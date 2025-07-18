using MediatR;

namespace LinkUp.Application;

public record AddUserToChatCommand(
    Guid ChatId,
    Guid AddUserId

) : IRequest<Guid>;