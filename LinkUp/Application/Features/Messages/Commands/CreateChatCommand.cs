using LinkUp.Domain;
using MediatR;

namespace LinkUp.Application;

public record CreateChatCommand(
    string Name,
    Guid CreatorId
) : IRequest<Guid>;