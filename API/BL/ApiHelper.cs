using Microsoft.AspNetCore.Authentication;

public static class ApiHelper
{
    public static void AddMyApiMethods(this WebApplication app)
    {
        // ===== Example Secured Endpoint =====
        app.MapGet(
                "/secure",
                (HttpContext context) =>
                {
                    var name = context.User.Identity?.Name ?? "Unknown";
                    return Results.Ok($"Hello, {name}!");
                }
            )
            .RequireAuthorization();

        app.MapGet("/", () => "Public Endpoint");

        // ===== Trigger Google Login =====
        app.MapGet(
            "/login",
            () =>
                Results.Challenge(
                    new AuthenticationProperties { RedirectUri = "/" },
                    new[] { "Google" }
                )
        );

        // ===== Logout =====
        app.MapGet(
            "/logout",
            async (HttpContext context) =>
            {
                await context.SignOutAsync("Cookies");
                return Results.Redirect("/");
            }
        );
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
            .WithName("GetWeatherForecast");
    }

    record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
