using DemoWebApi.Data;
using DemoWebApi.Models;
using LightTrace;

namespace DemoWebApi.Business;

public interface IUserService
{
    Task<User> GetUserAsync(int id);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User> CreateUserAsync(User user);
    Task<User> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> ValidateUserEmailAsync(string email);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> GetUserAsync(int id)
    {
        using var tracer = new Tracer($"UserService.GetUserAsync(id: {id})");
        
        if (id <= 0)
            throw new ArgumentException("User ID must be greater than 0", nameof(id));

        var user = await _userRepository.GetUserByIdAsync(id);
        
        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found");

        return user;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        using var tracer = new Tracer("UserService.GetAllUsersAsync");
        
        var users = await _userRepository.GetAllUsersAsync();
        
        // Simulate some business logic processing
        await Task.Delay(10);
        
        return users.OrderBy(u => u.Name);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        using var tracer = new Tracer($"UserService.CreateUserAsync(name: {user?.Name})");
        
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (string.IsNullOrWhiteSpace(user.Name))
            throw new ArgumentException("User name is required", nameof(user));

        if (string.IsNullOrWhiteSpace(user.Email))
            throw new ArgumentException("User email is required", nameof(user));

        // Validate email format and uniqueness
        if (!await ValidateUserEmailAsync(user.Email))
            throw new ArgumentException("Invalid or duplicate email address", nameof(user));

        return await _userRepository.CreateUserAsync(user);
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        using var tracer = new Tracer($"UserService.UpdateUserAsync(id: {user?.Id})");
        
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (user.Id <= 0)
            throw new ArgumentException("User ID must be greater than 0", nameof(user));

        // Check if user exists
        var existingUser = await _userRepository.GetUserByIdAsync(user.Id);
        if (existingUser == null)
            throw new KeyNotFoundException($"User with ID {user.Id} not found");

        if (string.IsNullOrWhiteSpace(user.Name))
            throw new ArgumentException("User name is required", nameof(user));

        if (string.IsNullOrWhiteSpace(user.Email))
            throw new ArgumentException("User email is required", nameof(user));

        return await _userRepository.UpdateUserAsync(user);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        using var tracer = new Tracer($"UserService.DeleteUserAsync(id: {id})");
        
        if (id <= 0)
            throw new ArgumentException("User ID must be greater than 0", nameof(id));

        // Check if user exists
        var existingUser = await _userRepository.GetUserByIdAsync(id);
        if (existingUser == null)
            throw new KeyNotFoundException($"User with ID {id} not found");

        return await _userRepository.DeleteUserAsync(id);
    }

    public async Task<bool> ValidateUserEmailAsync(string email)
    {
        using var tracer = new Tracer($"UserService.ValidateUserEmailAsync(email: {email})");
        
        if (string.IsNullOrWhiteSpace(email))
            return false;

        // Simulate email validation logic
        await Task.Delay(20);
        
        // Basic email format validation
        if (!email.Contains("@") || !email.Contains("."))
            return false;

        // Check for duplicate emails
        var allUsers = await _userRepository.GetAllUsersAsync();
        return !allUsers.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }
}