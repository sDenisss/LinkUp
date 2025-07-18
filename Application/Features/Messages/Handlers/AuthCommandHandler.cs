using LinkUp.Application;
using LinkUp.Domain;
using LinkUp.Infrastructure;
using LinkUp.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Application;

public class AuthCommandHandler : IRequestHandler<AuthCommand, string>
{
    private readonly ApplicationDbContext _db;
    private readonly IJwtTokenGenerator _jwt;

    public AuthCommandHandler(ApplicationDbContext db, IJwtTokenGenerator jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    public async Task<string> Handle(AuthCommand request, CancellationToken cancellationToken)
    {
        // var user = await _db.Users
        //     .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email.Value == request.Email.Value, cancellationToken);

        if (user == null)
        {
            throw new ApplicationException("User not found.");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.HashedPassword))
        {
            throw new ApplicationException("Invalid credentials.");
        }

        var token = _jwt.GenerateToken(user);
        return token;
    }
}
