using SATPrep.Api.Entities;
using SATPrep.Api.Utilities;

namespace SATPrep.Api.Data.DataSeeder;

public static class UserSeeder
{
    public static async Task Seed(AppDbContext context)
    {
        if (context.Users.Any()) return;

        var pwd = PasswordHasher.Hash("Test123!");

        var users = new List<User>
        {
            new() { Name = "Ali Valiyev", Email = "ali1@gmail.com", Password = pwd, Role = Role.User, CreatedAt = DateTime.UtcNow },
            new() { Name = "Bekzod Karimov", Email = "bekzod@gmail.com", Password = pwd, Role = Role.User, CreatedAt = DateTime.UtcNow },
            new() { Name = "Sardor Toshmatov", Email = "sardor@gmail.com", Password = pwd, Role = Role.User, CreatedAt = DateTime.UtcNow },
            new() { Name = "Jasur Aliyev", Email = "jasur@gmail.com", Password = pwd, Role = Role.User, CreatedAt = DateTime.UtcNow },
            new() { Name = "Aziza Nazarova", Email = "aziza@gmail.com", Password = pwd, Role = Role.User, CreatedAt = DateTime.UtcNow },
            new() { Name = "Madina Ergasheva", Email = "madina@gmail.com", Password = pwd, Role = Role.User, CreatedAt = DateTime.UtcNow },
            new() { Name = "Dilshod Rasulov", Email = "dilshod@gmail.com", Password = pwd, Role = Role.User, CreatedAt = DateTime.UtcNow },
            new() { Name = "Umid Xolmatov", Email = "umid@gmail.com", Password = pwd, Role = Role.User, CreatedAt = DateTime.UtcNow },
            new() { Name = "Shahnoza Qodirova", Email = "shahnoza@gmail.com", Password = pwd, Role = Role.User, CreatedAt = DateTime.UtcNow },
            new() { Name = "Malika Saidova", Email = "malika@gmail.com", Password = pwd, Role = Role.User, CreatedAt = DateTime.UtcNow },
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
    }
}