using LinkUp.Application;
using LinkUp.Domain;
using LinkUp.Persistence;
using MediatR;

public class GetUniqueNameByIdCommandHandler : IRequestHandler<GetUniqueNameByIdQuery, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ApplicationDbContext _context;

    public GetUniqueNameByIdCommandHandler(IUserRepository userRepository, ApplicationDbContext context)
    {
        _userRepository = userRepository;
        _context = context;
    }

    public async Task<UserDto> Handle(GetUniqueNameByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);

        return new UserDto(user.Id, user.Username, user.UniqueName);
    }

}