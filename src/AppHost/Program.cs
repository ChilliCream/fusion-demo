var builder = DistributedApplication.CreateBuilder(args);

builder.AddGraphQLOrchestrator();

var postgres = builder.AddPostgres("postgres");
var redis = builder.AddRedis("redis");

var accountsApi = builder
    .AddProject<Projects.Demo_Accounts>("accounts-api")
    .WithReference(postgres.AddDatabase("accounts-db"))
    .WithGraphQLSchemaFile()
    .WaitFor(postgres);

var inventoryApi = builder
    .AddProject<Projects.Demo_Inventory>("inventory-api")
    .WithReference(postgres.AddDatabase("inventory-db"))
    .WithGraphQLSchemaFile()
    .WaitFor(postgres);

var orderApi = builder
    .AddProject<Projects.Demo_Order>("order-api")
    .WithReference(postgres.AddDatabase("order-db"))
    .WithGraphQLSchemaEndpoint()
    .WaitFor(postgres);

var paymentsApi = builder
    .AddProject<Projects.Demo_Payments>("payments-api")
    .WithReference(postgres.AddDatabase("payments-db"))
    .WithGraphQLSchemaEndpoint(path: "/graphql?SDL")
    .WaitFor(postgres);

var productsApi = builder
    .AddProject<Projects.Demo_Products>("products-api")
    .WithReference(postgres.AddDatabase("products-db"))
    .WithGraphQLSchemaEndpoint()
    .WaitFor(postgres);

var reviewsApi = builder
    .AddProject<Projects.Demo_Reviews>("reviews-api")
    .WithReference(postgres.AddDatabase("reviews-db"))
    .WithReference(redis)
    .WithGraphQLSchemaEndpoint()
    .WaitFor(postgres)
    .WaitFor(redis);

var shippingApi = builder
    .AddProject<Projects.Demo_Shipping>("shipping-api")
    .WithGraphQLSchemaEndpoint();

builder
    .AddProject<Projects.Demo_Gateway>("gateway-api")
    .WithGraphQLSchemaComposition()
    .WithReference(accountsApi)
    .WithReference(inventoryApi)
    .WithReference(orderApi)
    .WithReference(paymentsApi)
    .WithReference(productsApi)
    .WithReference(reviewsApi)
    .WithReference(shippingApi);

builder.Build().Run();
