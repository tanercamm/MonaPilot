using MonaPilot.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MonaPilot.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<User> AddUserAsync(User user)
        {
            // In a real application, you would hash the password here
            // user.PasswordHash = HashPassword(user.Password);
            user.CreatedAt = System.DateTime.UtcNow;
            user.UpdatedAt = System.DateTime.UtcNow;
            return await _userRepository.AddUserAsync(user);
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            // In a real application, if password is being updated, hash it here
            user.UpdatedAt = System.DateTime.UtcNow;
            return await _userRepository.UpdateUserAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteUserAsync(id);
        }
    }
}
