using LinkUp.Application;
using LinkUp.Domain;

// namespace LinkUp.Persistence;

// public class InMemoryUserRepository : IUserRepository
// {
//     private static readonly Dictionary<Guid, User> _users = new Dictionary<Guid, User>();

//     public Task<User?> GetByIdAsync(Guid id)
//     {
//         _users.TryGetValue(id, out var user);
//         return Task.FromResult(user);
//     }

//     public Task AddAsync(User user)
//     {
//         if (_users.ContainsKey(user.Id))
//         {
//             throw new InvalidOperationException($"User with ID {user.Id} already exists.");
//         }
//         _users.Add(user.Id, user);
//         return Task.CompletedTask;
//     }

//     Task<User> IUserRepository.GetByIdAsync(Guid userId)
//     {
//         throw new NotImplementedException();
//     }

//     public Task<User> GetByUsernameAsync(string username)
//     {
//         throw new NotImplementedException();
//     }

//     public Task<User> GetByEmailAsync(string email)
//     {
//         throw new NotImplementedException();
//     }

//     public Task UpdateAsync(User user)
//     {
//         throw new NotImplementedException();
//     }
// }
