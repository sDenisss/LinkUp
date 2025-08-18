using LinkUp.Application;
using LinkUp.Domain;
using MediatR;

public class GetAllMessagesQueryHandler : IRequestHandler<GetAllMessagesQuery, IEnumerable<MessageDto>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;

    public GetAllMessagesQueryHandler(IMessageRepository messageRepository, IUserRepository userRepository)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<MessageDto>> Handle(GetAllMessagesQuery request, CancellationToken cancellationToken)
    {
        // 1. Вызываем метод репозитория, используя request.ChatId
        // Предполагается, что GetMessagesByIdAsync в IMessageRepository
        // на самом деле возвращает IEnumerable<Message> и фильтрует по ChatId.
        // Если вы переименовали его в GetMessagesByChatIdAsync, используйте его.
        var messages = await _messageRepository.GetMessagesByChatIdAsync(request.ChatId, cancellationToken);
        var senderIds = messages.Select(m => m.SenderId).Distinct().ToList();

        var users = await _userRepository.GetByIdsAsync(senderIds, cancellationToken);
        var userDict = users.ToDictionary(u => u.Id, u => u.UniqueName);

        // 2. Маппим коллекцию доменных сущностей Message в коллекцию MessageDto.
        // .Select() для трансформации каждого элемента.
        // .ToList() для материализации результатов в список.
        return messages.Select(m => new MessageDto(
            m.Id,
            m.SenderId,
            userDict.TryGetValue(m.SenderId, out var name) ? name : "Unknown",
            m.Content,
            m.Timestamp
        )).ToList();
    }
}