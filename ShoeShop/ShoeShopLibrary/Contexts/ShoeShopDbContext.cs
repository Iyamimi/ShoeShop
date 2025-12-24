using Microsoft.EntityFrameworkCore;
using ShoeShopLibrary.Models;

namespace ShoeShopLibrary.Contexts;

// Основной класс контекста базы данных для приложения ShoeShop сгенерирован автоматически через EF Core Scaffold
public partial class ShoeShopDbContext : DbContext
{
    public ShoeShopDbContext()
    {
    }

    public ShoeShopDbContext(DbContextOptions<ShoeShopDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    // Внимание: строка подключения должна быть вынесена в конфигурацию
    // Временная строка подключения для разработки
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost\\SQLEXPRESS;;Initial Catalog=ShoeShopDb;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0BC5EBA9A2");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64D8C7E8FFCE");

            entity.Property(e => e.FullName).HasMaxLength(150);
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.ManufacturerId).HasName("PK__Manufact__357E5CC15BA12071");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCF5CBAE89B");

            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Customers");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.Article });

            entity.Property(e => e.Article).HasMaxLength(20);

            entity.HasOne(d => d.ArticleNavigation).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.Article)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItems_Products");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItems_Orders");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Article);

            entity.Property(e => e.Article).HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Photo).HasMaxLength(500);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Unit).HasMaxLength(10);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Categories");

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Products)
                .HasForeignKey(d => d.ManufacturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Manufacturers");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Suppliers");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A28B412B1");

            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE666B428103D88");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.Login).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
