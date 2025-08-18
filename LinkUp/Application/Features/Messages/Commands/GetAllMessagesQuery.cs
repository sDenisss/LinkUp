using MediatR;

namespace LinkUp.Application;

public record GetAllMessagesQuery(
    Guid ChatId
) : IRequest<IEnumerable<MessageDto>>;