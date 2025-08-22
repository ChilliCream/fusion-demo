var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var redis = builder.AddRedis("redis");

var accountsApi = builder
    .AddProject<Projects.Demo_Accounts>("accounts-api")
    .WithReference(postgres.AddDatabase("accounts-db"))
    .WaitFor(postgres);

var inventoryApi = builder
    .AddProject<Projects.Demo_Inventory>("inventory-api")
    .WithReference(postgres.AddDatabase("inventory-db"))
    .WaitFor(postgres);

var orderApi = builder
    .AddProject<Projects.Demo_Order>("order-api")
    .WithReference(postgres.AddDatabase("order-db"))
    .WaitFor(postgres);

var paymentsApi = builder
    .AddProject<Projects.Demo_Payments>("payments-api")
    .WithReference(postgres.AddDatabase("payments-db"))
    .WaitFor(postgres);

var productsApi = builder
    .AddProject<Projects.Demo_Products>("products-api")
    .WithReference(postgres.AddDatabase("products-db"))
    .WaitFor(postgres);

var reviewsApi = builder
    .AddProject<Projects.Demo_Reviews>("reviews-api")
    .WithReference(postgres.AddDatabase("reviews-db"))
    .WithReference(redis)
    .WaitFor(postgres)
    .WaitFor(redis);

var shippingApi = builder
    .AddProject<Projects.Demo_Shipping>("shipping-api");

builder
    .AddProject<Projects.Demo_Gateway>("gateway-api")
    .WithReference(accountsApi)
    .WithReference(inventoryApi)
    .WithReference(orderApi)
    .WithReference(paymentsApi)
    .WithReference(productsApi)
    .WithReference(reviewsApi)
    .WithReference(shippingApi);

builder.Build().Run();
