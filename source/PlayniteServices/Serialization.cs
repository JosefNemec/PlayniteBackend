using Playnite.Common;
using Playnite.SDK;
using Playnite.SDK.Models;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace PlayniteServices;

public class ReleaseDateConverter : JsonConverter<ReleaseDate>
{
    public override ReleaseDate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return ReleaseDate.Deserialize(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, ReleaseDate value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Serialize());
    }
}

// TODO: use shared code from Playnite repo after .NET 7 migration is finished
public static class DataSerialization
{
    private static readonly ILogger logger = LogManager.GetLogger();

    private static readonly JsonSerializerOptions defaultOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        WriteIndented = true,
        IncludeFields = true,
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new ReleaseDateConverter()
        }
    };

    public static string ToJson<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, defaultOptions);
    }

    public static T FromJson<T>(string input)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(input, defaultOptions);
        }
        catch (Exception e)
        {
            logger.Error(e, $"Failed to deserialize {typeof(T).FullName} from JSON:");
            logger.Debug(input);
            throw;
        }
    }

    public static T FromJson<T>(Stream input)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(input, defaultOptions);
        }
        catch (Exception e)
        {
            logger.Error(e, $"Failed to deserialize {typeof(T).FullName} from JSON:");
            throw;
        }
    }

    public static T FromYaml<T>(string input) where T : class
    {
        try
        {
            var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
            return deserializer.Deserialize<T>(input);
        }
        catch (Exception e)
        {
            logger.Error(e, $"Failed to deserialize {typeof(T).FullName} from YAML:");
            logger.Debug(input);
            throw;
        }
    }

    public static T FromYamlFile<T>(string filePath) where T : class
    {
        return FromYaml<T>(FileSystem.ReadStringFromFile(filePath));
    }

    public static T GetCopy<T>(this T source) where T : class
    {
        if (source is null)
        {
            return null;
        }

        return FromJson<T>(ToJson(source));
    }
}