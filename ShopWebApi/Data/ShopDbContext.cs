using Microsoft.EntityFrameworkCore;
using ShopWebApi.Models;

namespace ShopWebApi.Data
{
    public class ShopDbContext(DbContextOptions<ShopDbContext> options) : DbContext(options)
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customers");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.BirthDate)
                    .IsRequired();
                entity.Property(e => e.RegistrationDate)
                    .IsRequired();
            });
            
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Article)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Price)
                    .IsRequired()
                    .HasColumnType("numeric(10,2)");
                entity.HasIndex(e => e.Article)
                    .IsUnique();
            });
            
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderNumber)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.OrderDate)
                    .IsRequired();
                entity.Property(e => e.TotalCost)
                    .IsRequired()
                    .HasColumnType("numeric(10,2)");
                entity.Property(e => e.CustomerId)
                    .IsRequired();
                entity.HasIndex(e => e.OrderNumber)
                    .IsUnique();
                
                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItems");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderId)
                    .IsRequired();
                entity.Property(e => e.ProductId)
                    .IsRequired();
                entity.Property(e => e.Quantity)
                    .IsRequired();
                
                entity.HasOne(e => e.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}