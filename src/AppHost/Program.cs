var builder = DistributedApplication.CreateBuilder(args);

builder.AddGraphQLOrchestrator();

var postgres = builder.AddPostgres("postgres");

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

builder
    .AddProject<Projects.Demo_Gateway>("gateway-api")
    .WithGraphQLSchemaComposition(
        settings: new GraphQLCompositionSettings
        {
            EnableGlobalObjectIdentification = true,
            EnvironmentName = "aspire"
        })
    .WithReference(productsApi);

builder.Build().Run();
