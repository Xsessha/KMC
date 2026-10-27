namespace Pr10Variant12;

internal sealed class AppOptions
{
    public string Host { get; private set; } = "127.0.0.1";
    public bool HostWasSpecified { get; private set; }
    public int? Port { get; private set; }
    public string Node { get; private set; } = "A12";
    public string Voltage { get; private set; } = "3.3";
    public string Current { get; private set; } = "0.25";
    public long Timestamp { get; private set; } = 1793145600;
    public double TimeoutSeconds { get; private set; } = 4.0;

    public static AppOptions Parse(string[] args)
    {
        var result = new AppOptions();
        for (var index = 0; index < args.Length; index++)
        {
            var key = args[index];
            if (!key.StartsWith("--", StringComparison.Ordinal))
                throw new ArgumentException($"Очікувався параметр --назва, отримано: {key}");

            if (index + 1 >= args.Length || args[index + 1].StartsWith("--", StringComparison.Ordinal))
                throw new ArgumentException($"Для параметра {key} не задано значення.");

            var value = args[++index];
            switch (key)
            {
                case "--host":
                    result.Host = value;
                    result.HostWasSpecified = true;
                    break;
                case "--port":
                    if (!int.TryParse(value, out var port) || port is < 1 or > 65535)
                        throw new ArgumentException("Порт має бути цілим числом у діапазоні 1–65535.");
                    result.Port = port;
                    break;
                case "--node":
                    if (string.IsNullOrWhiteSpace(value) || value.Contains(';') || value.Contains('='))
                        throw new ArgumentException("node має бути непорожнім і не містити ';' або '='.");
                    result.Node = value;
                    break;
                case "--v":
                    result.Voltage = ValidateFieldValue("--v", value);
                    break;
                case "--i":
                    result.Current = ValidateFieldValue("--i", value);
                    break;
                case "--ts":
                    if (!long.TryParse(value, out var timestamp) || timestamp < 0)
                        throw new ArgumentException("ts має бути невід’ємним цілим числом.");
                    result.Timestamp = timestamp;
                    break;
                case "--timeout":
                    if (!double.TryParse(value, System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture, out var timeout)
                        || timeout <= 0 || timeout > 300)
                        throw new ArgumentException("--timeout має бути числом у діапазоні (0; 300].");
                    result.TimeoutSeconds = timeout;
                    break;
                default:
                    throw new ArgumentException($"Невідомий параметр: {key}");
            }
        }
        return result;
    }

    private static string ValidateFieldValue(string option, string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Contains(';') || value.Contains('='))
            throw new ArgumentException($"{option} має бути непорожнім і не містити ';' або '='.");
        return value;
    }
}
