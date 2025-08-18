using LinkUp.Domain;

namespace LinkUp.UnitTests
{
    public class UserTests
    {
        [Fact]
        public void CreateUser_Should_SetPropertiesCorrectly()
        {
            // Arrange
            var email = new Email("denis@example.com");

            // Act
            var user = new User("Denis", email, "hashedpasswordverylong", "denis123");

            // Assert
            user.Username.Equals("Denis");
            user.UniqueName.Equals("denis123");
            user.Email.Value.Equals("denis@example.com");
            user.Id.Equals("hashedpasswordverylong");
        }
    }
}
    