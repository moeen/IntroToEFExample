using Spectre.Console;
using IntroToEFExample.DataService;
using IntroToEFExample.Models;
using Microsoft.EntityFrameworkCore;

namespace IntroToEFExample
{
    public static class Program
    {
        private static async Task Main()
        {
            var dbPath = GetDbPath();

            await using var ctx = new ApplicationDbContext(dbPath);

            Migrate(ctx);

            AnsiConsole.Write(new Rule("Intro to Entity Framework") { Justification = Justify.Left });
            AnsiConsole.MarkupLine($"SQLite database address: [blue]{dbPath}[/]");
            AnsiConsole.WriteLine("");

            var choices = new List<string> { "Use Entity Framework", "Use Plain Queries", "Exit" };
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("[yellow]Select the way to interact with database[/]")
                .AddChoices(choices));

            Func<ApplicationDbContext, IDataService> choiceFunc = choice switch
            {
                { } when choice == choices[0] => EfDataService,
                { } when choice == choices[1] => AdoDataService,
                _ => _ =>
                {
                    DeleteDb(dbPath);
                    Console.Clear();
                    Environment.Exit(0);
                    return null;
                }
            };

            var svc = choiceFunc(ctx);

            await RunExamples(svc);

            DeleteDb(dbPath);
        }

        private static string GetDbPath()
        {
            var path = Path.GetTempPath();
            return Path.Join(path, $"app-{Guid.NewGuid().ToString()}.db");
        }

        private static async void Migrate(DbContext ctx)
        {
            await ctx.Database.MigrateAsync();
        }

        private static EfDataService EfDataService(ApplicationDbContext ctx)
        {
            return new EfDataService(ctx);
        }

        private static AdoDataService AdoDataService(ApplicationDbContext ctx)
        {
            return new AdoDataService(ctx.Database.GetConnectionString() ?? "");
        }

        private static void DeleteDb(string dbPath)
        {
            File.Delete(dbPath);
        }

        private static async Task RunExamples(IDataService svc)
        {
            var choices = new List<string>
            {
                "Insert Predefined Data",
                "Display All Users",
                "Display All Products",
                "Create a random Order",
                "Get User Orders",
                "Get All Orders",
                "Exit"
            };

            var loop = true;
            while (loop)
            {
                AnsiConsole.Console.Clear();

                var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .Title("[yellow]Select the operation[/]")
                    .AddChoices(choices));

                Func<Task> action = choice switch
                {
                    { } when choice == choices[0] => async () =>
                    {
                        await svc.InsertPredefinedData();
                        AnsiConsole.MarkupLine("[green]Predefined data inserted successfully.[/]");
                    },
                    { } when choice == choices[1] => async () =>
                    {
                        var users = await svc.GetAllUsers();

                        users.ForEach(user =>
                            AnsiConsole.MarkupLine("User: [blue]{0}[/], Email: [blue]{1}[/]", user.Name, user.Email));
                    },
                    { } when choice == choices[2] => async () =>
                    {
                        var products = await svc.GetAllProducts();

                        products.ForEach(product => AnsiConsole.MarkupLine(
                            "Product: [blue]{0}[/], Price: [green]{1:C}[/]", product.Name,
                            product.Price));
                    },
                    { } when choice == choices[3] => async () =>
                    {
                        var rnd = new Random();

                        var allUsers = await svc.GetAllUsers();
                        var allProducts = await svc.GetAllProducts();

                        var randomUser = allUsers[rnd.Next(allUsers.Count)];

                        var randomProducts = allProducts.OrderBy(p => rnd.Next())
                            .Take(rnd.Next(1, allProducts.Count))
                            .ToList();

                        await svc.CreateOrder(randomUser.Id, randomProducts.Select(p => p.Id).ToList());
                        AnsiConsole.MarkupLine(
                            $"Order created successfully for [blue]{randomUser.Name}[/], Email: [blue]{randomUser.Email}[/]. Products added:");
                        randomProducts.ForEach(product => AnsiConsole.MarkupLine($"\t[green]{product.Name}[/]"));
                    },
                    { } when choice == choices[4] => async () =>
                    {
                        var users = await svc.GetAllUsers();
                        var user = AnsiConsole.Prompt(new SelectionPrompt<User>()
                            .Title("Select a user")
                            .AddChoices(users));

                        AnsiConsole.MarkupLine($"Orders for [green]{user.Name}[/]:");
                        AnsiConsole.WriteLine("");

                        var orders = await svc.GetUserOrdersWithOrderDetails(user.Id);

                        orders.ForEach(order => AnsiConsole.MarkupLine(
                            "Order Date: [blue]{0}[/], Total Items: [green]{1}[/]",
                            order.OrderDate,
                            order.OrderDetails.Count));
                    },
                    { } when choice == choices[5] => async () =>
                    {
                        var allOrders = await svc.GetAllOrderWithOrderDetailsAndUser();

                        allOrders.ForEach(order => AnsiConsole.MarkupLine(
                            "Order for [blue]{0}[/] on [blue]{1}[/] with [green]{2}[/] items.",
                            order.User.Name, order.OrderDate, order.OrderDetails.Count));
                    },
                    _ => () =>
                    {
                        loop = false;
                        Console.Clear();
                        return Task.CompletedTask;
                    }
                };

                await action();
                Console.ReadLine();
            }
        }
    }
}