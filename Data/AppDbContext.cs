using Microsoft.EntityFrameworkCore;
using TheNoir.Api.Models;

namespace TheNoir.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Topping> Toppings => Set<Topping>();
    public DbSet<OrderItemTopping> OrderItemToppings => Set<OrderItemTopping>();
    public DbSet<OrderMessage> OrderMessages => Set<OrderMessage>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<NullableUtcDateTimeConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Category>().HasIndex(c => c.Name).IsUnique();

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.City)
            .WithMany()
            .HasForeignKey(o => o.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<OrderItemTopping>()
            .HasOne(oit => oit.OrderItem)
            .WithMany(oi => oi.Toppings)
            .HasForeignKey(oit => oit.OrderItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItemTopping>()
            .HasOne(oit => oit.Topping)
            .WithMany()
            .HasForeignKey(oit => oit.ToppingId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<OrderMessage>()
            .HasOne(m => m.Order)
            .WithMany(o => o.Messages)
            .HasForeignKey(m => m.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
