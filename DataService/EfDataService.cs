using IntroToEFExample.Models;
using Microsoft.EntityFrameworkCore;


namespace IntroToEFExample.DataService;

public class EfDataService(ApplicationDbContext context) : IDataService
{
    public async Task InsertPredefinedData()
    {
        var users = new List<User>
        {
            new() { Name = "Ali", Email = "ali@example.com" },
            new() { Name = "Hossein", Email = "hossein@example.com" },
            new() { Name = "Mohammad", Email = "mohammad@example.com" },
            new() { Name = "Ahmad", Email = "ahmad@example.com" }
        };
        var products = new List<Product>
        {
            new() { Name = "Laptop", Price = 1000.00m },
            new() { Name = "Speaker", Price = 150.00m },
            new() { Name = "Headphone", Price = 200.00m },
            new() { Name = "Smartphone", Price = 500.00m }
        };

        await context.Users.AddRangeAsync(users);
        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }

    public async Task<List<User>> GetAllUsers()
    {
        return await context.Users.ToListAsync();
    }

    public async Task<List<Product>> GetAllProducts()
    {
        return await context.Products.ToListAsync();
    }

    public async Task CreateOrder(int userId, List<int> productIds)
    {
        var order = new Order { UserId = userId, OrderDate = DateTime.Now };
        foreach (var productId in productIds)
        {
            order.OrderDetails.Add(new OrderDetail { OrderId = order.Id, ProductId = productId });
        }

        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();
    }

    public async Task<List<Order>> GetUserOrdersWithOrderDetails(int userId)
    {
        return await context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.User)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .ToListAsync();
    }

    public async Task<List<Order>> GetAllOrderWithOrderDetailsAndUser()
    {
        return await context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .OrderBy(o => o.UserId)
            .ToListAsync();
    }
}