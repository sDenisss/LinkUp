using LinkUp.Domain;
using MediatR;

namespace LinkUp.Application;

public record GetAllChatsQuery(
    Guid UserId
) : IRequest<IEnumerable<ChatDto>>;