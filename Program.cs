using System.Text;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using TheNoir.Api.Data;
using TheNoir.Api.Models;
using TheNoir.Api.OpenApi;
using TheNoir.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var account = new Account(
        config["Cloudinary:CloudName"],
        config["Cloudinary:ApiKey"],
        config["Cloudinary:ApiSecret"]);
    return new Cloudinary(account);
});

builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IToppingService, ToppingService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddHttpClient<IEmailService, EmailService>();
builder.Services.AddHostedService<OrderAutoCancelService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateLifetime = true,
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddControllers();

// Comma-separated list in config/env (e.g. Cors__AllowedOrigins on the host),
// falling back to the local Next.js dev server so `dotnet run` still works out of the box.
var allowedOrigins = (builder.Configuration["Cors:AllowedOrigins"] ?? "http://localhost:3000")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddOpenApi(options =>
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

    // Applies pending EF Core migrations on boot so a fresh deploy (e.g. a new
    // Fly.io volume) gets a schema without anyone running `dotnet ef` by hand.
    await db.Database.MigrateAsync();

    if (!await db.Users.AnyAsync(u => u.Email == "admin"))
    {
        var now = DateTime.UtcNow;
        var admin = new User
        {
            Email = "admin",
            DisplayName = "Admin",
            PasswordHash = "",
            Role = UserRoles.Admin,
            CreatedAt = now,
            UpdatedAt = now,
        };
        // Falls back to "admin" for local dev; production sets Seed__AdminPassword
        // so a wiped/ephemeral disk never reseeds a publicly-known default password.
        var adminPassword = builder.Configuration["Seed:AdminPassword"] ?? "admin";
        admin.PasswordHash = hasher.HashPassword(admin, adminPassword);

        db.Users.Add(admin);
        await db.SaveChangesAsync();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // interactive API reference at /scalar
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // serves wwwroot/uploads/* for product images
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => new
{
    name = "Thé Noir API",
    endpoints = new[]
    {
        "/api/products", "/api/categories", "/api/cities", "/api/auth", "/api/uploads", "/api/orders", "/openapi/v1.json",
    },
});

app.MapControllers();

app.Run();
