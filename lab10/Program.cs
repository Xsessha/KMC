using Pr10Variant12;

if (args.Length == 0 || args[0] is "-h" or "--help" or "help")
{
    PrintHelp();
    return;
}

var mode = args[0].ToLowerInvariant();
var options = AppOptions.Parse(args.Skip(1).ToArray());

switch (mode)
{
    case "tcp-server":
        await TcpServerApp.RunAsync(options);
        break;
    case "tcp-client":
        await TcpClientApp.RunAsync(options);
        break;
    case "udp-server":
        await UdpServerApp.RunAsync(options);
        break;
    case "udp-client":
        await UdpClientApp.RunAsync(options);
        break;
    default:
        Console.Error.WriteLine($"Невідомий режим: {mode}");
        PrintHelp();
        Environment.ExitCode = 2;
        break;
}

static void PrintHelp()
{
    Console.WriteLine("""
    Практична №10 — варіант 12 (C# / .NET 8)
    Використання:
      dotnet run -- tcp-server [--host 0.0.0.0] [--port 6110]
      dotnet run -- tcp-client [--host 127.0.0.1] [--port 6110] [--node A12] [--v 3.3] [--i 0.25] [--ts 1793145600] [--timeout 4]
      dotnet run -- udp-server [--host 0.0.0.0] [--port 6111]
      dotnet run -- udp-client [--host 127.0.0.1] [--port 6111] [--node A12] [--v 3.3] [--i 0.25] [--ts 1793145600] [--timeout 3]
    """);
}
