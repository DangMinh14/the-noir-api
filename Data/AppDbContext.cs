using Microsoft.EntityFrameworkCore;
using TheNoir.Api.Features.Maisons;
using TheNoir.Api.Features.Products;

namespace TheNoir.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<City> Cities => Set<City>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Noir Signature",
                Category = "Milk Tea",
                Description = "Our house black tea folded into silky fresh milk, finished with burnt-caramel pearls.",
                PriceVnd = 65000,
                ImageUrl = "https://images.unsplash.com/photo-1558857563-b371033873b8?q=80&w=1200&auto=format&fit=crop",
                ImageAlt = "A glass of milk tea with tapioca pearls on a dark table",
                SortOrder = 1,
            },
            new Product
            {
                Id = 2,
                Name = "Highland Oolong",
                Category = "Pure Tea",
                Description = "Hand-rolled oolong from estates at 1,200 m: orchid nose, honeyed finish, and steep after steep.",
                PriceVnd = 58000,
                ImageUrl = "https://images.unsplash.com/photo-1564890369478-c89ca6d9cde9?q=80&w=1200&auto=format&fit=crop",
                ImageAlt = "Hot tea being poured from a teapot into a small cup",
                SortOrder = 2,
            },
            new Product
            {
                Id = 3,
                Name = "Phin Noir",
                Category = "Coffee",
                Description = "Đà Lạt arabica dripped through a traditional phin, layered over condensed milk.",
                PriceVnd = 55000,
                ImageUrl = "https://images.unsplash.com/photo-1509042239860-f550ce710b93?q=80&w=1200&auto=format&fit=crop",
                ImageAlt = "A dark cup of Vietnamese coffee in moody light",
                SortOrder = 3,
            },
            new Product
            {
                Id = 4,
                Name = "Jade Cloud",
                Category = "Matcha",
                Description = "Stone-ground ceremonial matcha whisked over cold jasmine milk, grassy and bright.",
                PriceVnd = 72000,
                ImageUrl = "https://images.unsplash.com/photo-1556679343-c7306c1976bc?q=80&w=1200&auto=format&fit=crop",
                ImageAlt = "An iced green matcha drink in a tall glass",
                SortOrder = 4,
            });

        modelBuilder.Entity<City>().HasData(
            new City { Id = 1, Name = "Sài Gòn", MaisonCount = 9, Kind = "Flagship", Address = "42 Vườn Trà", SortOrder = 1 },
            new City { Id = 2, Name = "Hà Nội", MaisonCount = 5, Kind = "Salon", Address = "18 Sương Mai", SortOrder = 2 },
            new City { Id = 3, Name = "Đà Nẵng", MaisonCount = 3, Kind = "Riverside", Address = "27 Bến Lá", SortOrder = 3 },
            new City { Id = 4, Name = "Huế", MaisonCount = 1, Kind = "Garden house", Address = "15 Đồi Mây", SortOrder = 4 });
    }
}
