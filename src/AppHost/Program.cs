var builder = DistributedApplication.CreateBuilder(args);

builder.AddNitro("dev");

var postgres = builder.AddPostgres("postgres");

// NATS backs the federated event streams. JetStream is enabled so the log is durable
// and clients can replay from an event cursor.
var nats = builder
    .AddNats("nats")
    .WithJetStream();

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
var promotionsDb = postgres.AddDatabase("promotions-db");

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
    .WithReference(nats)
    .WithGraphQLSchemaEndpoint()
    .WaitFor(postgres)
    .WaitFor(nats);

var shippingApi = builder
    .AddProject<Projects.Demo_Shipping>("shipping-api")
    .WithGraphQLSchemaEndpoint();

// The promotions API is a TypeScript source schema (graphql-yoga + graphql-federation-subgraph).
// The Aspire composition can only discover schema-settings.json for .NET project resources, so
// its schema reaches the gateway through Nitro instead; the "aspire" environment in its
// schema-settings.json routes the composed gateway back to this local resource on port 5118.
// The database is wired through the standard PG* variables that the node-postgres client
// reads natively, avoiding ADO.NET connection string parsing in TypeScript.
var postgresEndpoint = postgres.Resource.PrimaryEndpoint;
var postgresUser = postgres.Resource.UserNameParameter is { } userName
    ? ReferenceExpression.Create($"{userName}")
    : ReferenceExpression.Create($"postgres");

var promotionsApi = builder
    .AddJavaScriptApp("promotions-api", "../SourceSchemas/Promotions")
    .WithNpm()
    .WithHttpEndpoint(port: 5118, env: "PORT")
    .WithEnvironment("PGHOST", ReferenceExpression.Create($"{postgresEndpoint.Property(EndpointProperty.Host)}"))
    .WithEnvironment("PGPORT", ReferenceExpression.Create($"{postgresEndpoint.Property(EndpointProperty.Port)}"))
    .WithEnvironment("PGUSER", postgresUser)
    .WithEnvironment("PGPASSWORD", ReferenceExpression.Create($"{postgres.Resource.PasswordParameter}"))
    .WithEnvironment("PGDATABASE", promotionsDb.Resource.DatabaseName)
    .WaitFor(promotionsDb);

var cartApi = builder
    .AddProject<Projects.Demo_Cart>("cart-api")
    .WithReference(cartDb)
    .WithEnvironment("ConnectionStrings__cart_db", cartDb.Resource.ConnectionStringExpression)
    .WithGraphQLSchemaEndpoint()
    .WaitFor(postgres);

builder
    .AddProject<Projects.Demo_Gateway>("gateway-api")
    .WithNitroApiId("QXBpCmcwMTlkOTVlMmEwMTU3MDQwYWM1ZTdlODMxZWY0OTRmZQ==")
    .WithGraphQLSchemaComposition(
        settings: new GraphQLCompositionSettings
        {
            EnableGlobalObjectIdentification = true,
            EnvironmentName = "aspire"
        })
    .WithReference(keycloak)
    .WithEnvironment("Keycloak__Authority", keycloak.GetEndpoint("http"))
    .WithReference(nats)
    .WaitFor(nats)
    .WithReference(accountsApi)
    .WithReference(inventoryApi)
    .WithReference(orderApi)
    .WithReference(paymentsApi)
    .WithReference(productsApi)
    .WithReference(reviewsApi)
    .WithReference(shippingApi)
    .WithReference(cartApi)
    .WithReference(promotionsApi);

builder.Build().Run();
