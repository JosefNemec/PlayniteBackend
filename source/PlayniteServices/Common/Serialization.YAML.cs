using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace Playnite;

public partial class Serialization
{
    public static string ToYaml(object obj)
    {
        var serializer = new SerializerBuilder().Build();
        return serializer.Serialize(obj);
    }

    public static T FromYaml<T>(string yaml) where T : class
    {
        try
        {
            var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
            return deserializer.Deserialize<T>(yaml);
        }
        catch (Exception e)
        {
            logger.Error(e, $"Failed to deserialize {typeof(T).FullName} from yaml:");
            logger.Trace(yaml);
            throw;
        }
    }

    public static bool TryFromYaml<T>(string yaml, out T? deserialized, [NotNullWhen(false)] out Exception? error) where T : class
    {
        try
        {
            var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
            deserialized = deserializer.Deserialize<T>(yaml);
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

    public static T FromYamlFile<T>(string filePath) where T : class
    {
        return FromYaml<T>(FileSystem.ReadStringFromFile(filePath));
    }

    public static bool TryFromYamlFile<T>(string filePath, out T? deserialized, [NotNullWhen(false)] out Exception? error) where T : class
    {
        try
        {
            deserialized = FromYaml<T>(FileSystem.ReadStringFromFile(filePath));
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

    public static T FromYamlStream<T>(Stream stream) where T : class
    {
        using var sr = new StreamReader(stream, true);
        var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
        return deserializer.Deserialize<T>(sr);
    }
}
