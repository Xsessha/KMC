using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Pr10Variant12;

internal static class TcpServerApp
{
    public static async Task RunAsync(AppOptions options)
    {
        var port = options.Port ?? 6110;
        var listenHost = options.HostWasSpecified ? options.Host : "0.0.0.0";
        var address = ResolveListenAddress(listenHost);
        var listener = new TcpListener(address, port);
        listener.Start(5);
        Console.WriteLine($"[TCP server] Слухаю {address}:{port}");

        using (listener)
        {
            while (true)
            {
                TcpClient client;
                try
                {
                    client = await listener.AcceptTcpClientAsync();
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (SocketException ex)
                {
                    Console.Error.WriteLine($"[TCP server] Помилка accept: {ex.Message}");
                    continue;
                }

                _ = HandleClientAsync(client);
            }
        }
    }

    private static async Task HandleClientAsync(TcpClient client)
    {
        using (client)
        {
            var remote = client.Client.RemoteEndPoint;
            Console.WriteLine($"[TCP server] Підключився клієнт {remote}");
            try
            {
                await using var stream = client.GetStream();
                using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false,
                    bufferSize: 1024, leaveOpen: true);

                // Одне повідомлення завершується '\n'; клієнт надсилає саме такий рядок.
                var message = await reader.ReadLineAsync().WaitAsync(TimeSpan.FromSeconds(15));
                if (message is null)
                {
                    Console.WriteLine("[TCP server] Клієнт закрив з'єднання без повідомлення.");
                    return;
                }

                Console.WriteLine($"[TCP server] Отримано: {message}");
                try
                {
                    var fields = Protocol.ParseMessage(message);
                    Console.WriteLine("[TCP server] Поля: " +
                        string.Join(", ", fields.Select(pair => $"{pair.Key}={pair.Value}")));
                    await stream.WriteAsync(Encoding.UTF8.GetBytes("OK\n"));
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"[TCP server] Помилка формату: {ex.Message}");
                    await stream.WriteAsync(Encoding.UTF8.GetBytes($"ERROR: {ex.Message}\n"));
                }
            }
            catch (TimeoutException)
            {
                Console.WriteLine($"[TCP server] Тайм-аут читання від {remote}.");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[TCP server] Помилка вводу-виводу від {remote}: {ex.Message}");
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"[TCP server] Помилка сокета від {remote}: {ex.Message}");
            }
        }
    }

    private static IPAddress ResolveListenAddress(string host)
    {
        if (host == "0.0.0.0" || host == "*")
            return IPAddress.Any;
        if (IPAddress.TryParse(host, out var address))
            return address;
        throw new ArgumentException("Для сервера --host має бути IP-адресою, наприклад 0.0.0.0.");
    }
}
