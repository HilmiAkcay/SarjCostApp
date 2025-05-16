using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ====== CORS Setup ======
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    );
});

// Register Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

// ===== Google Auth =====
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "Cookies";
        options.DefaultChallengeScheme = "Google";
    })
    .AddCookie("Cookies")
    .AddGoogle(
        "Google",
        options =>
        {
            options.ClientId = builder.Configuration["Google:ClientId"];
            options.ClientSecret = builder.Configuration["Google:ClientSecret"];
        }
    );

builder.Services.AddAuthorization();

var app = builder.Build();

// Enable Swagger only in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

app.UseHttpsRedirection();

// Apply CORS policy
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// My Custom Method To Add Endpoints
app.AddMyApiMethods();

app.Run();
