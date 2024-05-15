using IntroToEFExample.Models;

namespace IntroToEFExample.DataService;

public interface IDataService
{
    Task InsertPredefinedData();
    Task<List<User>> GetAllUsers();
    Task<List<Product>> GetAllProducts();
    Task CreateOrder(int userId, List<int> productIds);
    Task<List<Order>> GetUserOrdersWithOrderDetails(int userId);
    Task<List<Order>> GetAllOrderWithOrderDetailsAndUser();
}