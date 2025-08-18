using LinkUp.Application;
using MediatR;

public class GetAllUsersInChatCommandHandler : IRequestHandler<GetAllUsersInChatQuery, IEnumerable<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersInChatCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersInChatQuery request, CancellationToken cancellationToken)
    {
        // 1. Вызываем метод репозитория, используя request.ChatId
        // Предполагается, что GetMessagesByIdAsync в IMessageRepository
        // на самом деле возвращает IEnumerable<Message> и фильтрует по ChatId.
        // Если вы переименовали его в GetMessagesByChatIdAsync, используйте его.
        var users = await _userRepository.GetUsersByChatIdAsync(request.ChatId, cancellationToken);

        // 2. Маппим коллекцию доменных сущностей Message в коллекцию MessageDto.
        // .Select() для трансформации каждого элемента.
        // .ToList() для материализации результатов в список.
        return users.Select(m => new UserDto(m.Id, m.Username, m.UniqueName)).ToList();
    }
}