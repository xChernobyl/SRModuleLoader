using System;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Phoenix.ModuleLoader
{
    public class JsonSerializationHelper
    {

        private static readonly JsonSerializerSettings DefaultSettings;

        static JsonSerializationHelper()
        {
            DefaultSettings =
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.None,
                };
        }

        public static bool IsJsonObject<T>() =>
           typeof(T).GetCustomAttribute<JsonObjectAttribute>() != default;

        public static bool TrySerialize<T>(
            out string result, in T obj, JsonSerializerSettings settings = default
            )
        {
            result = default;

            if (settings == null)
                settings = DefaultSettings;

            if (!IsJsonObject<T>())
                return false;

            try
            {
                result = JsonConvert.SerializeObject(obj, settings);
            }
            catch (JsonSerializationException serializationEx)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public static bool TryDeserialize<T>(
            out T result, string json, JsonSerializerSettings settings = default
            )
        {
            result = default;
            if (settings == null)
                settings = DefaultSettings;

            if (!IsJsonObject<T>())
                return false;

            try
            {
                result = JsonConvert.DeserializeObject<T>(json, settings);
            }
            catch (JsonSerializationException serializationEx)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public static bool TrySerializeToFile<T>(
            in T obj, string path, JsonSerializerSettings settings = default, Encoding encoding = default
            )
        {
            if (settings == default)
                settings = DefaultSettings;

            if (encoding == default)
                encoding = Encoding.Default;

            if (!IsJsonObject<T>())
                return false;

            if (!TrySerialize<T>(out string json, obj, settings))
                return false;

            try
            {
                File.WriteAllText(path, json, encoding);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public static bool TryDeserializeFromFile<T>(
            out T result, string path, JsonSerializerSettings settings = default, Encoding encoding = default
            )
        {
            result = default;
            if (settings == default)
                settings = DefaultSettings;

            if (encoding == default)
                encoding = Encoding.Default;

            if (!IsJsonObject<T>())
                return false;

            string fileContents;

            try
            {
                fileContents = File.ReadAllText(path, encoding);
            }
            catch (Exception ex)
            {
                return false;
            }

            return TryDeserialize<T>(out result, fileContents, settings);
        }
    }
}
