using LibWindPop.Packs.Pak;
using LibWindPop.Packs.Rsb;
using LibWindPop.Packs.Rsb.ContentPipeline;
using LibWindPop.Packs.Xpr;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Logger;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibWindPop.Utils
{
    public static class WindJsonSerializer
    {
        private static JsonSerializerOptions options;
        private static WindJsonSerializerContext context;

        static WindJsonSerializer()
        {
            options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
                IncludeFields = true,
            };
            options.Converters.Add(new JsonStringEnumConverter());
            context = new WindJsonSerializerContext(options);
        }

        public static void TrySerializeToFile<T>(string filePath, T value, uint version, IFileSystem fileSystem, ILogger logger, bool throwException)
            where T : class
        {
            try
            {
                using (Stream stream = fileSystem.Create(filePath))
                {
                    Serialize(stream, value, version);
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex, 0, throwException);
            }
        }

        public static T? TryDeserializeFromFile<T>(string filePath, uint version, IFileSystem fileSystem, ILogger logger, bool throwException)
            where T : class
        {
            T? value = null;
            try
            {
                using (Stream packInfoStream = fileSystem.OpenRead(filePath))
                {
                    value = Deserialize<T>(packInfoStream, version);
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex, 0, throwException);
                value = null;
            }
            return value;
        }

        public static void Serialize<T>(Stream stream, T value, uint version)
        {
            JsonSerializer.Serialize(stream, new WindJsonShell<T>
            {
                Source = typeof(T).AssemblyQualifiedName,
                Author = "YingFengTingYu",
                Version = version,
                Content = value
            }, typeof(WindJsonShell<T>), context);
        }

        public static T? Deserialize<T>(Stream stream, uint version)
        {
            return (JsonSerializer.Deserialize(stream, typeof(WindJsonShell<T>), context) is WindJsonShell<T> shell && shell.Version == version) ? shell.Content : default;
        }
    }

    internal struct WindJsonShell<T>
    {
        public string? Source;

        public string? Author;

        public uint? Version;

        public T? Content;
    }

    [JsonSourceGenerationOptions(
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        GenerationMode = JsonSourceGenerationMode.Metadata,
        IncludeFields = true,
        WriteIndented = true
        )]
    [JsonSerializable(typeof(WindJsonShell<RsbManifestInfo>))]
    [JsonSerializable(typeof(WindJsonShell<RsbPackInfo>))]
    [JsonSerializable(typeof(WindJsonShell<RsgMetadata>))]
    [JsonSerializable(typeof(WindJsonShell<PtxMetadata>))]
    [JsonSerializable(typeof(WindJsonShell<RsbContentPipelineInfo>))]
    [JsonSerializable(typeof(WindJsonShell<XprPackInfo>))]
    [JsonSerializable(typeof(WindJsonShell<PakPackInfo>))]
    internal partial class WindJsonSerializerContext : JsonSerializerContext
    {

    }
}
