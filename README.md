# IntroToEFExample

## Introduction
`IntroToEFExample` is a simple command-line interface (CLI) shopping application developed in .NET 8. This project is designed to demonstrate and compare the use of Entity Framework Core (EF Core) ORM versus plain SQL queries when interacting with a SQLite database. By implementing the same functionalities using both approaches, this project provides insights into the benefits and drawbacks of each method.

## Project Structure
The project includes two main implementations: one using Entity Framework Core and the other using plain SQL queries. Both implementations adhere to the following interface:

```csharp
public interface IDataService
{
    Task InsertPredefinedData();
    Task<List<User>> GetAllUsers();
    Task<List<Product>> GetAllProducts();
    Task CreateOrder(int userId, List<int> productIds);
    Task<List<Order>> GetUserOrdersWithOrderDetails(int userId);
    Task<List<Order>> GetAllOrderWithOrderDetailsAndUser();
}
```

## Features
- **InsertPredefinedData**: Inserts predefined data into the database.
- **GetAllUsers**: Retrieves a list of all users.
- **GetAllProducts**: Retrieves a list of all products.
- **CreateOrder**: Creates an order for a specified user with a list of product IDs.
- **GetUserOrdersWithOrderDetails**: Retrieves orders with detailed order information for a specified user.
- **GetAllOrderWithOrderDetailsAndUser**: Retrieves all orders along with detailed order information and associated user data.

## Prerequisites
- .NET 8 SDK
- SQLite

## Setup and Running the Project Locally

1. **Clone the Repository**
   ```bash
   git clone https://github.com/moeen/IntroToEFExample.git
   cd IntroToEFExample
   ```

2. **Build the Project**
   ```bash
   dotnet build
   ```

3. **Run the Project**
   ```bash
   dotnet run --project ./src/IntroToEFExample
   ```

4. **Select Implementation**
   During execution, you will be prompted to choose between the EF Core implementation and the Plain SQL implementation.

## Using the Application
Upon running the application, you will have access to a menu-driven interface where you can:
- Insert predefined data.
- List all users.
- List all products.
- Create an order.
- List user orders with details.
- List all orders with details and user information.

## Additional Resources
For more details on Entity Framework and a deeper understanding of the concepts demonstrated in this project, please refer to the presentation slides used in the project:

[Introduction to Entity Framework](./Introduction%20to%20Entity%20Framework.pdf)

### Summary of the Presentation:
- **Entity Framework Overview**: Introduction to ORM and EF Core.
- **Features of Entity Framework**: Benefits and key features.
- **CRUD Operations**: Examples of Create, Read, Update, and Delete operations using EF Core.
- **Comparison**: Pros and cons of using an ORM versus direct SQL queries.
- **Practical Example**: Demonstration using EF Core vs. plain queries.
