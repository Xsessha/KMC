using System.Net.Sockets;
using System.Text;

namespace Pr10Variant12;

internal static class UdpClientApp
{
    public static async Task RunAsync(AppOptions options)
    {
        var port = options.Port ?? 6111;
        var message = Protocol.BuildMessage(options);
        Console.WriteLine($"[UDP client] Надсилаю датаграму: {message}");

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(options.TimeoutSeconds));
        using var client = new UdpClient();
        try
        {
            await client.SendAsync(Encoding.UTF8.GetBytes(message), options.Host, port);
            var response = await client.ReceiveAsync(timeout.Token);
            var text = Protocol.Decode(response.Buffer, response.Buffer.Length);
            Console.WriteLine($"[UDP client] Відповідь від {response.RemoteEndPoint}: {text}");
            if (!text.Equals("OK", StringComparison.Ordinal))
                Environment.ExitCode = 1;
        }
        catch (OperationCanceledException)
        {
            Console.Error.WriteLine($"[UDP client] Тайм-аут: відповіді застосунку не отримано за {options.TimeoutSeconds:0.##} с. UDP сам по собі не підтверджує доставку.");
            Environment.ExitCode = 1;
        }
        catch (SocketException ex)
        {
            Console.Error.WriteLine($"[UDP client] Помилка сокета: {ex.SocketErrorCode} — {ex.Message}");
            Environment.ExitCode = 1;
        }
    }
}
