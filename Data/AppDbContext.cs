using Microsoft.EntityFrameworkCore;
using TheNoir.Api.Models;

namespace TheNoir.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<City> Cities => Set<City>();
}
