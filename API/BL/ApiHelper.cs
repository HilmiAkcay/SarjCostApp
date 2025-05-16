using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi; // Needed for WithOpenApi extensions

public static class ApiHelper
{
    public static void AddMyApiMethods(this WebApplication app)
    {
        // ===== Secured Endpoint =====
        app.MapGet(
                "/secure",
                (HttpContext context) =>
                {
                    var name = context.User.Identity?.Name ?? "Unknown";
                    return Results.Ok($"Hello, {name}!");
                }
            )
            .RequireAuthorization()
            .WithName("SecureEndpoint")
            .WithOpenApi()
            .WithSummary("Returns user info if authenticated")
            .WithDescription(
                "This endpoint returns the authenticated user's name. Requires Google login."
            );

        // ===== Public Endpoint =====
        app.MapGet("/", () => "Public Endpoint")
            .WithName("PublicEndpoint")
            .WithOpenApi()
            .WithSummary("Returns a public message")
            .WithDescription("This is a public endpoint accessible without authentication.");

        // ===== Google Login =====
        app.MapGet(
                "/login",
                () =>
                    Results.Challenge(
                        new AuthenticationProperties { RedirectUri = "/" },
                        new[] { "Google" }
                    )
            )
            .WithName("LoginWithGoogle")
            .WithOpenApi()
            .WithSummary("Initiates Google OAuth login")
            .WithDescription(
                "Redirects the user to Google for authentication. Returns to '/' on success."
            );

        // ===== Logout =====
        app.MapGet(
                "/logout",
                async (HttpContext context) =>
                {
                    await context.SignOutAsync("Cookies");
                    return Results.Redirect("/");
                }
            )
            .WithName("Logout")
            .WithOpenApi()
            .WithSummary("Logs out the current user")
            .WithDescription(
                "Signs out the current user from the cookie session and redirects to '/'."
            );

        // ===== Weather Forecast =====
        var summaries = new[]
        {
            "Freezing",
            "Bracing",
            "Chilly",
            "Cool",
            "Mild",
            "Warm",
            "Balmy",
            "Hot",
            "Sweltering",
            "Scorching",
        };

        app.MapGet(
                "/weatherforecast",
                () =>
                {
                    var forecast = Enumerable
                        .Range(1, 5)
                        .Select(index => new WeatherForecast(
                            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                            Random.Shared.Next(-20, 55),
                            summaries[Random.Shared.Next(summaries.Length)]
                        ))
                        .ToArray();

                    return forecast;
                }
            )
            .WithName("GetWeatherForecast")
            .WithOpenApi()
            .WithSummary("Returns a 5-day weather forecast")
            .WithDescription("Generates and returns dummy weather data for testing.");
    }

    record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
