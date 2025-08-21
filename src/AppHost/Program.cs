var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");

var accountsDb = postgres.AddDatabase("accounts-db");
var accountsApi = builder.AddProject<Projects.Demo_Accounts>("accounts-api");

var inventoryDb = postgres.AddDatabase("inventory-db");
var inventoryApi = builder.AddProject<Projects.Demo_Inventory>("inventory-api");

var orderDb = postgres.AddDatabase("order-db");
var orderApi = builder.AddProject<Projects.Demo_Order>("order-api");

var paymentsDb = postgres.AddDatabase("payments-db");
var paymentsApi = builder.AddProject<Projects.Demo_Payments>("payments-api");

var productsDb = postgres.AddDatabase("products-db");
var productsApi = builder.AddProject<Projects.Demo_Products>("products-api");

var reviewsDb = postgres.AddDatabase("reviews-db");
var reviewsApi = builder.AddProject<Projects.Demo_Reviews>("reviews-api");

builder.Build().Run();
