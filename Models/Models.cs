using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IntroToEFExample.Models;

public class ApplicationDbContext(string dbPath) : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={dbPath}");
    }
}

public class User
{
    public int Id { get; set; }
    [Column(TypeName = "TEXT")] public string Name { get; set; }
    [Column(TypeName = "TEXT")] public string Email { get; set; }

    public List<Order> Orders { get; set; } = [];

    public override string ToString()
    {
        return $"{Id}. {Name} - {Email}";
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

[Index(nameof(User.Id))]
public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public User User { get; set; }
    public List<OrderDetail> OrderDetails { get; set; } = [];
}

public class OrderDetail
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public Order Order { get; set; }
    public Product Product { get; set; }
}