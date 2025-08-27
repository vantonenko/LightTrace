using DemoWebApi.Models;
using LightTrace;

namespace DemoWebApi.Data;

public interface IUserRepository
{
    Task<User> GetUserByIdAsync(int id);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User> CreateUserAsync(User user);
    Task<User> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(int id);
}

public class UserRepository : IUserRepository
{
    // Simulated in-memory database
    private static readonly List<User> Users = new()
    {
        new User { Id = 1, Name = "John Doe", Email = "john@example.com", CreatedAt = DateTime.UtcNow.AddDays(-30) },
        new User { Id = 2, Name = "Jane Smith", Email = "jane@example.com", CreatedAt = DateTime.UtcNow.AddDays(-20) },
        new User { Id = 3, Name = "Bob Johnson", Email = "bob@example.com", CreatedAt = DateTime.UtcNow.AddDays(-10) }
    };

    public async Task<User> GetUserByIdAsync(int id)
    {
        using var tracer = new Tracer($"UserRepository.GetUserByIdAsync(id: {id})");
        
        // Simulate database query delay
        await Task.Delay(50);
        
        return Users.FirstOrDefault(u => u.Id == id);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        using var tracer = new Tracer("UserRepository.GetAllUsersAsync");
        
        // Simulate database query delay
        await Task.Delay(100);
        
        return Users.ToList();
    }

    public async Task<User> CreateUserAsync(User user)
    {
        using var tracer = new Tracer($"UserRepository.CreateUserAsync(name: {user.Name})");
        
        // Simulate database insert delay
        await Task.Delay(75);
        
        user.Id = Users.Max(u => u.Id) + 1;
        user.CreatedAt = DateTime.UtcNow;
        Users.Add(user);
        
        return user;
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        using var tracer = new Tracer($"UserRepository.UpdateUserAsync(id: {user.Id})");
        
        // Simulate database update delay
        await Task.Delay(60);
        
        var existingUser = Users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser != null)
        {
            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            return existingUser;
        }
        
        return null;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        using var tracer = new Tracer($"UserRepository.DeleteUserAsync(id: {id})");
        
        // Simulate database delete delay
        await Task.Delay(40);
        
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user != null)
        {
            Users.Remove(user);
            return true;
        }
        
        return false;
    }
}