using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TheNoir.Api.Data;
using TheNoir.Api.Features.Maisons;
using TheNoir.Api.Features.Products;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // interactive API reference at /scalar
}

app.UseHttpsRedirection();
app.UseCors();

app.MapGet("/", () => new
{
    name = "Thé Noir API",
    endpoints = new[] { "/api/products", "/api/maisons", "/openapi/v1.json" },
});

app.MapProducts();
app.MapMaisons();

app.Run();
