var builder = DistributedApplication.CreateBuilder(args);

builder.AddGraphQLOrchestrator();

var postgres = builder.AddPostgres("postgres");

var keycloak = builder
    .AddKeycloak("keycloak", port: 8080)
    .WithRealmImport("./fusion-demo-realm.json");

var accountsDb = postgres.AddDatabase("accounts-db");
var inventoryDb = postgres.AddDatabase("inventory-db");
var orderDb = postgres.AddDatabase("order-db");
var paymentsDb = postgres.AddDatabase("payments-db");
var productsDb = postgres.AddDatabase("products-db");
var reviewsDb = postgres.AddDatabase("reviews-db");
var cartDb = postgres.AddDatabase("cart-db");

var accountsApi = builder
    .AddProject<Projects.Demo_Accounts>("accounts-api")
    .WithReference(accountsDb)
    .WithEnvironment("ConnectionStrings__accounts_db", accountsDb.Resource.ConnectionStringExpression)
    .WithGraphQLSchemaEndpoint()
    .WaitFor(postgres);

var inventoryApi = builder
    .AddProject<Projects.Demo_Inventory>("inventory-api")
    .WithReference(inventoryDb)
    .WithEnvironment("ConnectionStrings__inventory_db", inventoryDb.Resource.ConnectionStringExpression)
    .WithGraphQLSchemaEndpoint()
    .WaitFor(postgres);

var orderApi = builder
    .AddProject<Projects.Demo_Order>("order-api")
    .WithReference(orderDb)
    .WithEnvironment("ConnectionStrings__order_db", orderDb.Resource.ConnectionStringExpression)
    .WithGraphQLSchemaEndpoint()
    .WaitFor(postgres);

var paymentsApi = builder
    .AddProject<Projects.Demo_Payments>("payments-api")
    .WithReference(paymentsDb)
    .WithEnvironment("ConnectionStrings__payments_db", paymentsDb.Resource.ConnectionStringExpression)
    .WithGraphQLSchemaEndpoint()
    .WaitFor(postgres);

var productsApi = builder
    .AddProject<Projects.Demo_Products>("products-api")
    .WithReference(productsDb)
    .WithEnvironment("ConnectionStrings__products_db", productsDb.Resource.ConnectionStringExpression)
    .WithGraphQLSchemaEndpoint()
    .WaitFor(postgres);

var reviewsApi = builder
    .AddProject<Projects.Demo_Reviews>("reviews-api")
    .WithReference(reviewsDb)
    .WithEnvironment("ConnectionStrings__reviews_db", reviewsDb.Resource.ConnectionStringExpression)
    .WithGraphQLSchemaEndpoint()
    .WaitFor(postgres);

var shippingApi = builder
    .AddProject<Projects.Demo_Shipping>("shipping-api")
    .WithGraphQLSchemaEndpoint();

var cartApi = builder
    .AddProject<Projects.Demo_Cart>("cart-api")
    .WithReference(cartDb)
    .WithEnvironment("ConnectionStrings__cart_db", cartDb.Resource.ConnectionStringExpression)
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
