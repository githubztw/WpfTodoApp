using Microsoft.EntityFrameworkCore;
using WpfTodoApp.Core.Models;

namespace WpfTodoApp.Services.Data;

/// <summary>
/// 应用数据库上下文。使用 SQLite 存储 Users 和 TodoItems。
/// 首次创建时自动 seed admin 用户（密码 123456 的 BCrypt 哈希）。
/// </summary>
public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    private readonly string _dbPath = null!;

    public AppDbContext()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "WpfTodoApp");
        Directory.CreateDirectory(folder);
        _dbPath = Path.Combine(folder, "app.db");
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured)
            options.UseSqlite($"Data Source={_dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Username).IsUnique();
            e.Property(u => u.Username).HasMaxLength(50).IsRequired();
            e.Property(u => u.PasswordHash).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<TodoItem>(e =>
        {
            e.Property(t => t.Title).HasMaxLength(200).IsRequired();
            e.Property(t => t.Description).HasMaxLength(1000);
            e.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(t => t.UserId);
        });

        // Seed default admin user
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            IsAdmin = true,
            IsFirstLogin = true
        });
    }
}
