using System.Collections.ObjectModel;
using HotChocolate;
using HotChocolate.Execution;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpClient("fusion");

builder.Services
    .AddGraphQLGatewayServer()
    .AddFileSystemConfiguration("./gateway.graphql")
    .AddHttpClientConfiguration("CATALOG", new Uri("http://localhost:5095/graphql"))
    .AddHttpClientConfiguration("REVIEWS", new Uri("http://localhost:5091/graphql"))
    .ModifyRequestOptions(o =>
    {
        o.CollectOperationPlanTelemetry = true;
    })
    .ConfigureSchemaServices((_, s) => s.AddSingleton<IErrorFilter, ErrorInfoFilter>());

var app = builder.Build();

app.MapGraphQL();

app.Run();

public class ErrorInfoFilter : IErrorFilter
{
    public IError OnError(IError error)
    {
        if (error.Exception is not null)
        {
            return error
                .WithMessage(error.Exception.Message)
                .WithExtensions(
                    new Dictionary<string, object?>
                    {
                        { "stackTrace", error.Exception.StackTrace }
                    });
        }
        
        return error;
    }
}
