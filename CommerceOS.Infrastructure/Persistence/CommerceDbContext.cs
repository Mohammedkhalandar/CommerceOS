using CommerceOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommerceOS.Infrastructure.Persistence;

public class CommerceDbContext : DbContext
{
    public CommerceDbContext(DbContextOptions<CommerceDbContext> options)
        : base(options)
    {
    }

    // ======================================================
    // CUSTOMER
    // ======================================================

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Address> Addresses => Set<Address>();

    // ======================================================
    // AUTHENTICATION
    // ======================================================

    public DbSet<User> Users => Set<User>();

    // ======================================================
    // PRODUCT
    // ======================================================

    public DbSet<Product> Products => Set<Product>();

    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();

    public DbSet<Category> Categories => Set<Category>();

    // ======================================================
    // CART
    // ======================================================

    public DbSet<Cart> Carts => Set<Cart>();

    public DbSet<CartItem> CartItems => Set<CartItem>();

    // ======================================================
    // INVENTORY
    // ======================================================

    public DbSet<Inventory> Inventories => Set<Inventory>();

    public DbSet<InventoryReservation> InventoryReservations
        => Set<InventoryReservation>();

    // ======================================================
    // ORDERS
    // ======================================================

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    // ======================================================
    // PAYMENTS
    // ======================================================

    public DbSet<Payment> Payments => Set<Payment>();

    // ======================================================
    // MODEL CONFIGURATION
    // ======================================================

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(CommerceDbContext).Assembly
        );
    }
}