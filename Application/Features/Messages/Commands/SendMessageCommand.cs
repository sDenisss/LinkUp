using MediatR;

namespace LinkUp.Application;
public record SendMessageCommand(
    // Guid Id,
    Guid ChatId,
    Guid SenderId,
    string Content
) : IRequest<Guid>;
