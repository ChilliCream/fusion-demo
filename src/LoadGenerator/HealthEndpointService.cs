using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LoadGenerator;

internal sealed class HealthEndpointService(ILogger<HealthEndpointService> logger)
    : BackgroundService
{
    private const int DefaultPort = 8080;
    private static readonly byte[] s_response = CreateResponse();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var port = int.TryParse(
            Environment.GetEnvironmentVariable("WEBSITES_PORT"),
            out var configuredPort)
            ? configuredPort
            : DefaultPort;

        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        logger.LogInformation("Health endpoint listening on port {Port}", port);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync(stoppingToken);
                _ = RespondAsync(client, stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Normal application shutdown.
        }
        finally
        {
            listener.Stop();
        }
    }

    private static async Task RespondAsync(
        TcpClient client,
        CancellationToken cancellationToken)
    {
        using (client)
        {
            try
            {
                var stream = client.GetStream();
                using var reader = new StreamReader(
                    stream,
                    Encoding.ASCII,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: 1024,
                    leaveOpen: true);

                while (await reader.ReadLineAsync(cancellationToken) is { Length: > 0 })
                {
                    // Consume the request headers before closing the connection.
                }

                await stream.WriteAsync(s_response, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Normal application shutdown.
            }
            catch (IOException)
            {
                // The probe disconnected before reading the response.
            }
        }
    }

    private static byte[] CreateResponse()
    {
        const string body = "{\"status\":\"healthy\"}";
        var response =
            "HTTP/1.1 200 OK\r\n" +
            "Content-Type: application/json\r\n" +
            $"Content-Length: {Encoding.UTF8.GetByteCount(body)}\r\n" +
            "Connection: close\r\n" +
            "\r\n" +
            body;

        return Encoding.UTF8.GetBytes(response);
    }
}
