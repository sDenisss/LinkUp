using LinkUp.Application;
using MediatR;

public class GetAllChatsQueryHandler : IRequestHandler<GetAllChatsQuery, IEnumerable<ChatDto>>
{
    private readonly IChatRepository _chatRepository;

        public GetAllChatsQueryHandler(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        // Corrected Handle method signature and implementation
        public async Task<IEnumerable<ChatDto>> Handle(GetAllChatsQuery request, CancellationToken cancellationToken)
        {
            // Use the userId from the request to get all chats for that user
            var chats = await _chatRepository.GetAllAsync(request.UserId, cancellationToken);

            // Project the Chat entities into ChatDto
            return chats.Select(c => new ChatDto(c.Id, c.Name)).ToList();
        }
}