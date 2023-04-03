using System.Text;

namespace PlayniteServices.Utilities;

public class IgdbProtoParser
{
    int currentPos = 0;
    char[] content = new char[1];

    public string ReadToken()
    {
        var value = new StringBuilder();
        var chr = content[currentPos];
        if (currentPos + 1 >= content.Length)
        {
            return string.Empty;
        }

        while (char.IsWhiteSpace(chr))
        {
            currentPos++;
            chr = content[currentPos];
        }

        while (!char.IsWhiteSpace(chr))
        {
            value.Append(chr);
            currentPos++;
            chr = content[currentPos];
        }

        return value.ToString();
    }

    private Dictionary<string, string> typeToEndpoint = new Dictionary<string, string>
    {
        {  "AgeRating", "age_ratings" },
        {  "AgeRatingContentDescription", "age_rating_content_descriptions" },
        {  "AlternativeName", "alternative_names" },
        {  "Artwork", "artworks" },
        {  "Character", "characters" },
        {  "CharacterMugShot", "character_mug_shots" },
        {  "Collection", "collections" },
        {  "Company", "companies" },
        {  "CompanyLogo", "company_logos" },
        {  "CompanyWebsite", "company_websites" },
        {  "Cover", "covers" },
        {  "ExternalGame", "external_games" },
        {  "Franchise", "franchises" },
        {  "Game", "games" },
        {  "GameEngine", "game_engines" },
        {  "GameEngineLogo", "game_engine_logos" },
        {  "GameLocalization", "game_localizations" },
        {  "GameMode", "game_modes" },
        {  "GameVersion", "game_versions" },
        {  "GameVersionFeature", "game_version_features" },
        {  "GameVersionFeatureValue", "game_version_feature_values" },
        {  "GameVideo", "game_videos" },
        {  "Genre", "genres" },
        {  "InvolvedCompany", "involved_companies" },
        {  "Keyword", "keywords" },
        {  "Language", "languages" },
        {  "LanguageSupport", "language_supports" },
        {  "LanguageSupportType", "language_support_types" },
        {  "MultiplayerMode", "multiplayer_modes" },
        {  "Platform", "platforms" },
        {  "PlatformFamily", "platform_families" },
        {  "PlatformLogo", "platform_logos" },
        {  "PlatformVersion", "platform_versions" },
        {  "PlatformVersionCompany", "platform_version_companies" },
        {  "PlatformVersionReleaseDate", "platform_version_release_dates" },
        {  "PlatformWebsite", "platform_websites" },
        {  "PlayerPerspective", "player_perspectives" },
        {  "Region", "regions" },
        {  "ReleaseDate", "release_dates" },
        {  "Screenshot", "screenshots" },
        {  "Theme", "themes" },
        {  "Website", "websites" }
    };

    private Dictionary<string, string> typeConversions = new Dictionary<string, string>
    {
        {  "double", "double" },
        {  "float", "float" },
        {  "int32", "int" },
        {  "int64", "long" },
        {  "uint32", "uint" },
        {  "uint64", "ulong" },
        {  "sint32", "int" },
        {  "sint64", "long" },
        {  "fixed32", "uint" },
        {  "fixed64", "ulong" },
        {  "sfixed32", "int" },
        {  "sfixed64", "long" },
        {  "bool", "bool" },
        {  "string", "string?" },
        {  "bytes", "ByteString?" },
        {  "google.protobuf.Timestamp", "long" },
    };

    public class ProtoMessage
    {
        public string Name { get; set; } = string.Empty;
        public List<ProtoMessageProperty> Properties { get; set; } = new List<ProtoMessageProperty>();

        public override string ToString()
        {
            return Name;
        }
    }

    public class ProtoMessageProperty
    {
        public string Modified { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public uint Position { get; set; }
        public string Type { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Name} {Type}";
        }
    }

    public class ProtoEnum
    {
        public string Name { get; set; } = string.Empty;
        public List<ProtoEnumValue> Values { get; set; } = new List<ProtoEnumValue>();

        public override string ToString()
        {
            return Name;
        }
    }

    public class ProtoEnumValue
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Name} {Value}";
        }
    }

    private ProtoMessage ParseMessage()
    {
        var message = new ProtoMessage();

        var token = ReadToken();
        message.Name = token;
        var propertyFields = new List<string>();

        ReadToken(); // opening {
        while (token != "}")
        {
            propertyFields.Clear();
            do
            {
                token = ReadToken();
                if (token == "}")
                {
                    break;
                }

                propertyFields.Add(token);
            }
            while (!token.EndsWith(';'));

            if (propertyFields.Count == 4)
            {
                message.Properties.Add(new ProtoMessageProperty
                {
                    Type = propertyFields[0],
                    Name = propertyFields[1],
                    Position = uint.Parse(propertyFields[3].TrimEnd(';'))
                });
            }
            else if (propertyFields.Count == 5)
            {
                message.Properties.Add(new ProtoMessageProperty
                {
                    Modified = propertyFields[0],
                    Type = propertyFields[1],
                    Name = propertyFields[2],
                    Position = uint.Parse(propertyFields[4].TrimEnd(';'))
                });
            }
            else if (propertyFields.Count == 0)
            {
                break;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        return message;
    }

    private ProtoEnum ParseEnum()
    {
        var pEnum = new ProtoEnum();
        var token = ReadToken();
        pEnum.Name = token;

        ReadToken(); // opening {
        token = ReadToken();
        while (token != "}")
        {
            var enumValue = new ProtoEnumValue();
            enumValue.Name = token;
            ReadToken(); // =
            enumValue.Value = ReadToken().TrimEnd(';');
            pEnum.Values.Add(enumValue);
            token = ReadToken();
        }

        return pEnum;
    }

    public void ParseFile(string protoFile, string outputDir)
    {
        var messages = new List<ProtoMessage>();
        var enums = new List<ProtoEnum>();

        content = File.ReadAllText(protoFile, Encoding.UTF8).ToCharArray();
        currentPos = 0;

        while (currentPos <= content.Length)
        {
            var token = ReadToken();
            if (token == "message")
            {
                var message = ParseMessage();
                if (!message.Name.EndsWith("Result", StringComparison.Ordinal) &&
                    message.Name != "TestDummy" &&
                    message.Name != "Search" &&
                    message.Name != "Count" &&
                    message.Name != "MultiQueryResultArray")
                {
                    messages.Add(message);
                }
            }
            else if (token == "enum")
            {
                var pEnum = ParseEnum();
                if (pEnum.Name != "TestDummyEnumTestEnum")
                {
                    enums.Add(pEnum);
                }
            }
            else if (token == string.Empty)
            {
                break;
            }
            else
            {
                throw new NotSupportedException(token);
            }
        }

        var result = new StringBuilder();
        result.AppendLine("namespace PlayniteServices.Controllers.IGDB;\r\n");

        foreach (var pEnum in enums)
        {
            result.AppendLine($"public enum {pEnum.Name}\r\n{{");
            foreach (var value in pEnum.Values)
            {
                result.AppendLine($"    {value.Name} = {value.Value},");
            }

            result.AppendLine("}\r\n");
        }

        foreach (var message in messages)
        {
            result.AppendLine($"public class {message.Name} : IgdbItem\r\n{{");
            foreach (var prop in message.Properties)
            {
                if (prop.Name == "id")
                {
                    continue;
                }

                var type = "ulong";
                if (typeConversions.TryGetValue(prop.Type, out var newType))
                {
                    type = newType;
                }

                var isEnum = enums.FirstOrDefault(a => a.Name == type);
                if (isEnum != null)
                {
                    type = isEnum.Name;
                }

                if (prop.Modified == "repeated")
                {
                    result.AppendLine($"    public List<{type.TrimEnd('?')}>? {prop.Name} {{ get; set; }}");
                }
                else if (string.IsNullOrEmpty(prop.Modified))
                {
                    result.AppendLine($"    public {type} {prop.Name} {{ get; set; }}");
                }
                else
                {
                    throw new NotImplementedException(prop.Modified);
                }
            }

            result.AppendLine("");

            foreach (var prop in message.Properties)
            {
                if (prop.Name == "id")
                {
                    continue;
                }

                var type = messages.FirstOrDefault(a => a.Name == prop.Type);
                if (type == null)
                {
                    continue;
                }

                if (prop.Modified == "repeated")
                {
                    result.AppendLine($"    public List<{type.Name.TrimEnd('?')}>? {prop.Name}_expanded {{ get; set; }}");
                }
                else if (string.IsNullOrEmpty(prop.Modified))
                {
                    result.AppendLine($"    public {type}? {prop.Name}_expanded {{ get; set; }}");
                }
                else
                {
                    throw new NotImplementedException(prop.Modified);
                }
            }

            result.AppendLine("");
            result.AppendLine("    public override string ToString()\r\n    {");
            if (message.Properties.Any(a => a.Name == "name"))
            {
                result.AppendLine("        return $\"{id}: {name}\";");
            }
            else
            {
                result.AppendLine("        return id.ToString();");
            }

            result.AppendLine("    }");
            result.AppendLine("}\r\n");
        }

        File.WriteAllText(Path.Combine(outputDir, "IgdbModels_Generated.cs"), result.ToString(), Encoding.UTF8);

        result = new StringBuilder();
        result.AppendLine("""
            using PlayniteServices.Controllers.IGDB;
            using MongoDB.Driver;
            using System.Diagnostics.CodeAnalysis;
            namespace PlayniteServices;
            public partial class IgdbApi : IDisposable
            {
            """);

        // Collection properties
        foreach (var message in messages)
        {
            result.AppendLine($"    [AllowNull] public {message.Name}Collection {message.Name}s {{ get; private set; }}");
        }

        // Collection init
        result.AppendLine("    void InitCollections()\r\n    {");
        foreach (var message in messages)
        {
            result.AppendLine($"       {message.Name}s = new {message.Name}Collection(this, Database);");
            result.AppendLine($"       DataCollections.Add({message.Name}s);");
        }
        result.AppendLine("    }");

        // Collection classes
        foreach (var message in messages)
        {
            result.AppendLine($$"""
            public class {{message.Name}}Collection : IgdbCollection<{{message.Name}}>
            {
                public {{message.Name}}Collection(IgdbApi igdb, Database database) : base(igdb, "{{typeToEndpoint[message.Name]}}", database)
                {
                }
            """);

            result.AppendLine($$"""
                public override void CreateIndexes()
                {
                """);

            if (message.Name == "Game")
            {
                result.AppendLine($$"""
                    collection.Indexes.CreateOne(new CreateIndexModel<{{message.Name}}>(Builders<{{message.Name}}>.IndexKeys.Text(x => x.name)));
                    """);
            }
            else if (message.Name == "ExternalGame")
            {
                result.AppendLine($$"""
                    collection.Indexes.CreateOne(new CreateIndexModel<{{message.Name}}>(Builders<{{message.Name}}>.IndexKeys.Ascending(x => x.uid)));
                    """);
            }
            else if (message.Name == "AlternativeName")
            {
                result.AppendLine($$"""
                    collection.Indexes.CreateOne(new CreateIndexModel<{{message.Name}}>(Builders<{{message.Name}}>.IndexKeys.Text(x => x.name)));
                    """);
            }
            else if (message.Name == "GameLocalization")
            {
                result.AppendLine($$"""
                    collection.Indexes.CreateOne(new CreateIndexModel<{{message.Name}}>(Builders<{{message.Name}}>.IndexKeys.Text(x => x.name)));
                    """);
            }

            result.AppendLine("}\r\n}");
        }

        result.AppendLine("}");

        File.WriteAllText(Path.Combine(outputDir, "IgdbApi_Generated.cs"), result.ToString(), Encoding.UTF8);

        result = new StringBuilder();
        result.AppendLine("""
            using Microsoft.AspNetCore.Mvc;

            namespace PlayniteServices.Controllers.IGDB;
            """);

        foreach (var message in messages)
        {
            var endpoint = typeToEndpoint[message.Name];
            result.AppendLine($$"""
            [Route("igdb/webhooks/{{endpoint}}")]
            public class {{message.Name}}WebhookController : WebhookController<{{message.Name}}>
            {
                public {{message.Name}}WebhookController(IgdbApi igdb, UpdatableAppSettings settings) : base("{{endpoint}}", igdb, settings)
                {
                }
            }
            """);
        }

        File.WriteAllText(Path.Combine(outputDir, "WebhookController_Generated.cs"), result.ToString(), Encoding.UTF8);
    }
}
