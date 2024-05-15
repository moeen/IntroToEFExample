using System.Data;
using IntroToEFExample.Models;
using Microsoft.Data.Sqlite;

namespace IntroToEFExample.DataService;

internal struct Person
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public class AdoDataService : IDataService
{
    private readonly SqliteConnection _connection;

    public AdoDataService(string connectionString)
    {
        _connection = new SqliteConnection(connectionString);
        _connection.Open();
    }

    ~AdoDataService()
    {
        _connection.Close();
    }

    public async Task InsertPredefinedData()
    {
        const string cmdText = """
                                   INSERT INTO Users (Name, Email) VALUES
                                       ('Ali', 'ali@example.com'),
                                       ('Hossein', 'hossein@example.com'),
                                       ('Mohammad', 'mohammad@example.com'),
                                       ('Ahmad', 'ahmad@example.com');
                                   INSERT INTO Products (Name, Price) VALUES
                                       ('Laptop', 1000.00),
                                       ('Speaker', 150.00),
                                       ('Headphone', 200.00),
                                       ('Smartphone', 500.00);
                               """;
        
        await using var command = new SqliteCommand(cmdText, _connection);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<User>> GetAllUsers()
    {
        var users = new List<User>();

        await using var command = new SqliteCommand("SELECT * FROM Users", _connection);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            users.Add(new User
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Email = reader.GetString(reader.GetOrdinal("Email"))
            });
        }

        return users;
    }

    public async Task<List<Product>> GetAllProducts()
    {
        var products = new List<Product>();

        await using var command = new SqliteCommand("SELECT * FROM Products", _connection);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            products.Add(new Product
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price"))
            });
        }

        return products;
    }

    public async Task CreateOrder(int userId, List<int> productIds)
    {
        var orderCommand = new SqliteCommand(
            "INSERT INTO Orders (UserId, OrderDate) VALUES (@UserId, @OrderDate) RETURNING Id",
            _connection);
        orderCommand.Parameters.AddWithValue("@UserId", userId);
        orderCommand.Parameters.AddWithValue("@OrderDate", DateTime.Now);

        var orderId = Convert.ToInt32(orderCommand.ExecuteScalar());

        foreach (var productId in productIds)
        {
            var x = _connection.CreateCommand();
            var detailCommand =
                new SqliteCommand("INSERT INTO OrderDetails (OrderId, ProductId) VALUES (@OrderId, @ProductId)",
                    _connection);
            detailCommand.Parameters.AddWithValue("@OrderId", orderId);
            detailCommand.Parameters.AddWithValue("@ProductId", productId);

            await detailCommand.ExecuteNonQueryAsync();
        }
    }

    public async Task<List<Order>> GetUserOrdersWithOrderDetails(int userId)
    {
        var orders = new List<Order>();

        var command =
            new SqliteCommand(
                "SELECT o.*, od.ProductId FROM Orders o JOIN OrderDetails od ON o.Id = od.OrderId WHERE o.UserId = @UserId",
                _connection);
        command.Parameters.AddWithValue("@UserId", userId);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var orderId = reader.GetInt32(reader.GetOrdinal("Id"));
            var orderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate"));
            var orderDetail = new OrderDetail
            {
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                OrderId = orderId
            };

            var order = orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                order = new Order
                {
                    Id = orderId,
                    UserId = userId,
                    OrderDate = orderDate,
                    OrderDetails = new List<OrderDetail> { orderDetail }
                };
                orders.Add(order);
            }
            else
            {
                order.OrderDetails.Add(orderDetail);
            }
        }

        return orders;
    }

    public async Task<List<Order>> GetAllOrderWithOrderDetailsAndUser()
    {
        var orders = new List<Order>();

        await using var command = new SqliteCommand(
            "SELECT o.*, u.Name, u.Email, od.ProductId FROM Orders o JOIN Users u ON o.UserId = u.Id JOIN OrderDetails od ON o.Id = od.OrderId ORDER BY o.UserId",
            _connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var orderId = reader.GetInt32(reader.GetOrdinal("Id"));
            var userId = reader.GetInt32(reader.GetOrdinal("UserId"));
            var user = new User
            {
                Id = userId,
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Email = reader.GetString(reader.GetOrdinal("Email"))
            };
            var orderDetail = new OrderDetail
            {
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                OrderId = orderId
            };

            var order = orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                order = new Order
                {
                    Id = orderId,
                    UserId = userId,
                    User = user,
                    OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                    OrderDetails = new List<OrderDetail> { orderDetail }
                };
                orders.Add(order);
            }
            else
            {
                order.OrderDetails.Add(orderDetail);
            }
        }

        return orders;
    }
}