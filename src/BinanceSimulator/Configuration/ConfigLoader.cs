using System.Text.Json;

namespace BinanceSimulator.Configuration;

public static class ConfigLoader
{
    public static AppConfig Load(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Configuration file '{path}' was not found.");
        }

        using FileStream stream = File.OpenRead(path);
        AppConfig? config = JsonSerializer.Deserialize<AppConfig>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        });

        return config ?? throw new InvalidOperationException("Failed to deserialize application configuration.");
    }
}
