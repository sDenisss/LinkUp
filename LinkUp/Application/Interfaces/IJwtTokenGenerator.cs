using LinkUp.Domain;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}