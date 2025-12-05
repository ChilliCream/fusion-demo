var builder = DistributedApplication.CreateBuilder(args);

builder.AddGraphQLOrchestrator();

var postgres = builder.AddPostgres("postgres");

var keycloak = builder
    .AddKeycloak("keycloak", port: 8080)
    .WithRealmImport("./fusion-demo-realm.json");

var accountsApi = builder
    .AddProject<Projects.Demo_Accounts>("accounts-api")
    .WithReference(postgres.AddDatabase("accounts-db"))
    .WithGraphQLSchemaEndpoint()
    .WaitFor(postgres);

var inventoryApi = builder
    .AddProject<Projects.Demo_Inventory>("inventory-api")
    .WithReference(postgres.AddDatabase("inventory-db"))
    .WithGraphQLSchemaEndpoint()
    .WaitFor(postgres);

var orderApi = builder
    .AddProject<Projects.Demo_Order>("order-api")
    .WithReference(postgres.AddDatabase("order-db"))
    .WithGraphQLSchemaEndpoint()
    .WaitFor(postgres);

var paymentsApi = builder
    .AddProject<Projects.Demo_Payments>("payments-api")
    .WithReference(postgres.AddDatabase("payments-db"))
    .WithGraphQLSchemaEndpoint()
    .WaitFor(postgres);

var productsApi = builder
    .AddProject<Projects.Demo_Products>("products-api")
    .WithReference(postgres.AddDatabase("products-db"))
    .WithGraphQLSchemaEndpoint()
    .WaitFor(postgres);

var reviewsApi = builder
    .AddProject<Projects.Demo_Reviews>("reviews-api")
    .WithReference(postgres.AddDatabase("reviews-db"))
    .WithGraphQLSchemaEndpoint()
    .WaitFor(postgres);

var shippingApi = builder
    .AddProject<Projects.Demo_Shipping>("shipping-api")
    .WithGraphQLSchemaEndpoint();

var cartApi = builder
    .AddProject<Projects.Demo_Cart>("cart-api")
    .WithReference(postgres.AddDatabase("cart-db"))
    .WithGraphQLSchemaEndpoint()
    .WaitFor(postgres);

builder
    .AddProject<Projects.Demo_Gateway>("gateway-api")
    .WithGraphQLSchemaComposition(
        settings: new GraphQLCompositionSettings
        {
            EnableGlobalObjectIdentification = true,
            EnvironmentName = "aspire"
        })
    .WithReference(keycloak)
    .WithEnvironment("Keycloak__Authority", keycloak.GetEndpoint("http"))
    .WithReference(accountsApi)
    .WithReference(inventoryApi)
    .WithReference(orderApi)
    .WithReference(paymentsApi)
    .WithReference(productsApi)
    .WithReference(reviewsApi)
    .WithReference(shippingApi)
    .WithReference(cartApi);

builder.Build().Run();
