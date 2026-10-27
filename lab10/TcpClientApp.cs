using System.Net.Sockets;
using System.Text;

namespace Pr10Variant12;

internal static class TcpClientApp
{
    public static async Task RunAsync(AppOptions options)
    {
        var port = options.Port ?? 6110;
        var message = Protocol.BuildMessage(options);
        Console.WriteLine($"[TCP client] Надсилаю: {message}");

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(options.TimeoutSeconds));
        using var client = new TcpClient();
        try
        {
            await client.ConnectAsync(options.Host, port, timeout.Token);
            await using var stream = client.GetStream();
            await stream.WriteAsync(Protocol.Encode(message), timeout.Token);

            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024, leaveOpen: true);
            var response = await reader.ReadLineAsync(timeout.Token);
            if (response is null)
            {
                Console.Error.WriteLine("[TCP client] Сервер закрив з'єднання без відповіді.");
                Environment.ExitCode = 1;
                return;
            }

            Console.WriteLine($"[TCP client] Відповідь: {response}");
            if (!response.Equals("OK", StringComparison.Ordinal))
                Environment.ExitCode = 1;
        }
        catch (OperationCanceledException)
        {
            Console.Error.WriteLine($"[TCP client] Тайм-аут після {options.TimeoutSeconds:0.##} с.");
            Environment.ExitCode = 1;
        }
        catch (SocketException ex)
        {
            Console.Error.WriteLine($"[TCP client] Помилка сокета: {ex.SocketErrorCode} — {ex.Message}");
            Environment.ExitCode = 1;
        }
        catch (IOException ex)
        {
            Console.Error.WriteLine($"[TCP client] Помилка вводу-виводу: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }
}
