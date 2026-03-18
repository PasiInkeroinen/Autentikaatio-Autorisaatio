using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthDemo.Models;
using AuthDemo.Services;

namespace AuthDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    // Open endpoint - no authentication required
    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    // Protected endpoint - requires authentication
    [Authorize]
    [HttpGet("secure")]
    public IActionResult GetSecure()
    {
        return Ok(new { Message = "This is a protected endpoint. You have been successfully authenticated!" });
    }

    // Admin endpoint - requires Admin role
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("admin")]
    public IActionResult GetAdminOnly()
    {
        return Ok(new { Message = "This is an admin-protected endpoint. You have the Admin role!" });
    }

    // Login endpoint - generates token
    [HttpPost("login")]
    public IActionResult Login([FromBody] UserCredentials credentials)
    {
        // Validate credentials
        if (credentials.Username == "testuser" && credentials.Password == "testpassword")
        {
            // Regular user - generate normal token
            var tokenService = new TokenService();
            var token = tokenService.GenerateToken(isAdmin: false);
            return Ok(new { Token = token });
        }
        else if (credentials.Username == "admin" && credentials.Password == "adminpassword")
        {
            // Admin user - generate admin token
            var tokenService = new TokenService();
            var token = tokenService.GenerateToken(isAdmin: true);
            return Ok(new { Token = token });
        }
        else
        {
            // Invalid credentials - return error
            return Unauthorized("Invalid username or password.");
        }
    }
}
