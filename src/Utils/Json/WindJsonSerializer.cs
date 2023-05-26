using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Logger;
using System;
using System.IO;
using System.Text.Json;

namespace LibWindPop.Utils.Json
{
    internal static class WindJsonSerializer
    {
        private static readonly JsonSerializerOptions options;
        private static readonly WindJsonSerializerContext context;

        static WindJsonSerializer()
        {
            options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
                IncludeFields = true,
            };
            context = new WindJsonSerializerContext(options);
        }

        public static void TrySerializeToFile<T>(string filePath, T value, IFileSystem fileSystem, ILogger logger)
            where T : class, IJsonVersionCheckable
        {
            try
            {
                using (Stream stream = fileSystem.Create(filePath))
                {
                    Serialize(stream, value);
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        public static T? TryDeserializeFromFile<T>(string filePath, IFileSystem fileSystem, ILogger logger)
            where T : class, IJsonVersionCheckable
        {
            T? value = null;
            try
            {
                using (Stream packInfoStream = fileSystem.OpenRead(filePath))
                {
                    value = Deserialize<T>(packInfoStream);
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                value = null;
            }
            return value;
        }

        public static void Serialize<T>(Stream stream, T value)
            where T : class, IJsonVersionCheckable
        {
            JsonSerializer.Serialize(stream, new WindJsonShell<T>
            {
                Source = typeof(T).AssemblyQualifiedName,
                Author = "YingFengTingYu",
                Version = T.JsonVersion,
                Content = value
            }, typeof(WindJsonShell<T>), context);
        }

        public static T? Deserialize<T>(Stream stream)
            where T : class, IJsonVersionCheckable
        {
            return (JsonSerializer.Deserialize(stream, typeof(WindJsonShell<T>), context) is WindJsonShell<T> shell && shell.Version == T.JsonVersion) ? shell.Content : default;
        }
    }
}
