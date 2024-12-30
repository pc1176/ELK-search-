using System.Net.Sockets;
using System.Text.Json;
namespace IVAEventGerator;

public class LogstashForwarderService : ILogstashForwarderService
{
    private readonly string _logstashHost;
    private readonly int _logstashPort;
    private readonly JsonSerializerOptions _jsonOptions;

    public LogstashForwarderService(string host = "localhost", int port = 5044)
    {
        _logstashHost = host;
        _logstashPort = port;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task ForwardEventAsync(IVAEvents ivaEvent)
    {
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(_logstashHost, _logstashPort);
            
            using var stream = client.GetStream();
            using var writer = new StreamWriter(stream);

            var jsonEvent = JsonSerializer.Serialize(ivaEvent, _jsonOptions);
            await writer.WriteLineAsync(jsonEvent);
            await writer.FlushAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error forwarding event to Logstash: {ex.Message}");
            throw;
        }
    }
}

public interface ILogstashForwarderService
{
    Task ForwardEventAsync(IVAEvents ivaEvent);
}