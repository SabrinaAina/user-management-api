using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

var users = new Dictionary<int, User>
{
    { 1, new User { Id = 1, Name = "Alice", Email = "alice@example.com" } },
    { 2, new User { Id = 2, Name = "Bob", Email = "bob@example.com" } }
};

var app = builder.Build();


// Error Handling Middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception)
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = "Internal server error." });
    }
});


// API Key Authentication Middleware
app.Use(async (context, next) =>
{
    if (!context.Request.Headers.TryGetValue("X-Api-Key", out var apiKey) ||
        apiKey != "TechHiveSecretAuthKey123")
    {
        context.Response.StatusCode = 401;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
        return;
    }

    await next();
});


// Logging Middleware
app.Use(async (context, next) =>
{
    await next();
    Console.WriteLine($"[Request] {context.Request.Method} {context.Request.Path} => {context.Response.StatusCode}");
});


// Routes
app.MapGet("/", () => Results.Ok("User Management API is running 🚀"));

app.MapGet("/users", () => Results.Ok(users.Values));

app.MapGet("/users/{id:int}", (int id) =>
{
    return users.TryGetValue(id, out var user)
        ? Results.Ok(user)
        : Results.NotFound(new { message = $"User with ID {id} not found." });
});

app.MapPost("/users", (User newUser) =>
{
    var validationResults = ValidationHelper.ValidateModel(newUser);
    if (validationResults.Any())
        return Results.BadRequest(new { errors = validationResults });

    if (users.ContainsKey(newUser.Id))
        return Results.Conflict(new { message = $"User with ID {newUser.Id} already exists." });

    users[newUser.Id] = newUser;
    return Results.Created($"/users/{newUser.Id}", newUser);
});

app.MapPut("/users/{id:int}", (int id, User updatedUser) =>
{
    var validationResults = ValidationHelper.ValidateModel(updatedUser);
    if (validationResults.Any())
        return Results.BadRequest(new { errors = validationResults });

    if (!users.ContainsKey(id))
        return Results.NotFound(new { message = $"User with ID {id} not found." });

    updatedUser.Id = id;
    users[id] = updatedUser;
    return Results.Ok(updatedUser);
});

app.MapDelete("/users/{id:int}", (int id) =>
{
    if (!users.ContainsKey(id))
        return Results.NotFound(new { message = $"User with ID {id} not found." });

    users.Remove(id);
    return Results.NoContent();
});

app.Run();


// User model with validation
public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [MinLength(2, ErrorMessage = "Name must be at least 2 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = string.Empty;
}

// Validation helper
public static class ValidationHelper
{
    public static List<string> ValidateModel(User user)
    {
        var context = new ValidationContext(user);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(user, context, results, true);
        return results.Select(r => r.ErrorMessage ?? "Validation error").ToList();
    }
}