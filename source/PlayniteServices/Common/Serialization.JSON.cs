using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Playnite;

public partial class Serialization
{
    private static readonly ILogger logger = LogManager.GetLogger();

    private static readonly JsonSerializerOptions jsonDesSettings = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
        IncludeFields = true,
        PropertyNameCaseInsensitive = true
    };

    public static string ToJson(object obj, bool formatted = false, JsonSerializerOptions? options = null)
    {
        options ??= new JsonSerializerOptions
        {
            WriteIndented = formatted,
            IncludeFields = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        return JsonSerializer.Serialize(obj, options);
    }

    public static T? FromJson<T>(string json) where T : class
    {
        try
        {
            return JsonSerializer.Deserialize<T>(json, jsonDesSettings);
        }
        catch (Exception e)
        {
            logger.Error(e, $"Failed to deserialize {typeof(T).FullName} from json:");
            logger.Trace(json);
            throw;
        }
    }

    public static T? FromJsonStream<T>(Stream stream) where T : class
    {
        try
        {
            return JsonSerializer.Deserialize<T>(stream, jsonDesSettings);
        }
        catch (Exception e)
        {
            logger.Error(e, $"Failed to deserialize {typeof(T).FullName} from json stream.");
            throw;
        }
    }

    public static bool TryFromJsonStream<T>(Stream stream, out T? deserialized, out Exception? error) where T : class
    {
        try
        {
            deserialized = JsonSerializer.Deserialize<T>(stream, jsonDesSettings);
            error = null;
            return true;
        }
        catch (Exception e)
        {
            deserialized = null;
            error = e;
            return false;
        }
    }

    public static T? FromJsonFile<T>(string filePath) where T : class
    {
        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return FromJsonStream<T>(fs);
    }

    public static bool TryFromJsonFile<T>(string filePath, out T? deserialized, [NotNullWhen(false)] out Exception? error) where T : class
    {
        try
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            deserialized = FromJsonStream<T>(fs);
            error = null;
            return true;
        }
        catch (Exception e)
        {
            deserialized = null;
            error = e;
            return false;
        }
    }

    public static bool TryFromJson<T>(string json, out T? deserialized, [NotNullWhen(false)] out Exception? error) where T : class
    {
        try
        {
            deserialized = JsonSerializer.Deserialize<T>(json, jsonDesSettings);
            error = null;
            return true;
        }
        catch (Exception e)
        {
            deserialized = null;
            error = e;
            return false;
        }
    }
}
