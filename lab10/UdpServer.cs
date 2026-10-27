using System.Net;
using System.Net.Sockets;

namespace Pr10Variant12;

internal static class UdpServerApp
{
    public static async Task RunAsync(AppOptions options)
    {
        var port = options.Port ?? 6111;
        var listenHost = options.HostWasSpecified ? options.Host : "0.0.0.0";
        var address = ResolveListenAddress(listenHost);
        using var server = new UdpClient(new IPEndPoint(address, port));
        Console.WriteLine($"[UDP server] Слухаю {address}:{port}");

        while (true)
        {
            UdpReceiveResult result;
            try
            {
                result = await server.ReceiveAsync();
            }
            catch (SocketException ex)
            {
                Console.Error.WriteLine($"[UDP server] Помилка приймання: {ex.Message}");
                continue;
            }

            var message = Protocol.Decode(result.Buffer, result.Buffer.Length);
            Console.WriteLine($"[UDP server] Від {result.RemoteEndPoint}: {message}");
            string reply;
            try
            {
                var fields = Protocol.ParseMessage(message);
                Console.WriteLine("[UDP server] Поля: " +
                    string.Join(", ", fields.Select(pair => $"{pair.Key}={pair.Value}")));
                reply = "OK\n";
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"[UDP server] Помилка формату: {ex.Message}");
                reply = $"ERROR: {ex.Message}\n";
            }

            try
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(reply);
                await server.SendAsync(bytes, result.RemoteEndPoint);
            }
            catch (SocketException ex)
            {
                Console.Error.WriteLine($"[UDP server] Помилка надсилання відповіді: {ex.Message}");
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
