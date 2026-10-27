using System.Text;

namespace Pr10Variant12;

internal static class Protocol
{
    private static readonly string[] RequiredFields = ["node", "v", "i", "ts"];

    public static string BuildMessage(AppOptions options) =>
        $"node={options.Node};v={options.Voltage};i={options.Current};ts={options.Timestamp}";

    public static Dictionary<string, string> ParseMessage(string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        var fields = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var rawPart in message.Trim().Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var separator = rawPart.IndexOf('=');
            if (separator <= 0)
                throw new FormatException($"Некоректне поле (очікується key=value): '{rawPart}'.");

            var key = rawPart[..separator].Trim();
            var value = rawPart[(separator + 1)..].Trim();
            if (key.Length == 0)
                throw new FormatException("Назва поля не може бути порожньою.");
            if (!fields.TryAdd(key, value))
                throw new FormatException($"Поле '{key}' повторюється.");
        }

        foreach (var required in RequiredFields)
        {
            if (!fields.ContainsKey(required))
                throw new FormatException($"Відсутнє обов’язкове поле '{required}'.");
        }

        return fields;
    }

    public static byte[] Encode(string message) => Encoding.UTF8.GetBytes(message + "\n");

    public static string Decode(byte[] data, int count) =>
        Encoding.UTF8.GetString(data, 0, count).Trim();
}
