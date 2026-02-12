using Microsoft.AspNetCore.Mvc;
using MonaPilot.API.Models;
using MonaPilot.API.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MonaPilot.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> AddUser(User user)
        {
            var newUser = await _userService.AddUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Update properties explicitly to avoid overwriting CreatedAt, etc.
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.PasswordHash = user.PasswordHash; // In a real app, handle password updates securely
            existingUser.UpdatedAt = System.DateTime.UtcNow;

            var updatedUser = await _userService.UpdateUserAsync(existingUser);
            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult>
DeleteUser(int id)
{
    var existingUser = await _userService.GetUserByIdAsync(id);
    if (existingUser == null)
    {
        return NotFound();
    }

    await _userService.DeleteUserAsync(id);
    return NoContent();
}
    }
}
