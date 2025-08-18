using LinkUp.Application;
using LinkUp.Domain;
using LinkUp.Persistence;
using MediatR;

public class GetByUniqueNameCommandHandler : IRequestHandler<GetByUniqueNameQuery, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ApplicationDbContext _context;

    public GetByUniqueNameCommandHandler(IUserRepository userRepository, ApplicationDbContext context)
    {
        _userRepository = userRepository;
        _context = context;
    }

    public async Task<UserDto> Handle(GetByUniqueNameQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUniqueNameAsync(request.UniqueName, cancellationToken);

        return new UserDto(user.Id, user.Username, user.UniqueName);
    }

}