using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Common.Helper.Extension
{
    public static class JsonExtension
    {
        public static string ToJson(this object obj, bool ignoreNullProp = true)
        {
            try
            {
                if (ignoreNullProp)
                {
                    var contractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    };

                    return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
                    {
                        ContractResolver = contractResolver,
                        Formatting = Formatting.Indented,
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }

                return JsonConvert.SerializeObject(obj);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static object? ToObject(this string strFullyQualifiedName)
        {
            var t = Type.GetType(strFullyQualifiedName);
            return t != null
                ? Activator.CreateInstance(t)
                : null;
        }

        public static T? ToModel<T>(this string? jsonString)
        {
            try
            {
                if (jsonString == null)
                {
                    return default;
                }

                var obj = JsonConvert.DeserializeObject<T>(jsonString);
                return obj;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static bool IsValidJson(this string? strInput)
        {
            if (string.IsNullOrEmpty(strInput) || string.IsNullOrWhiteSpace(strInput))
                return false;

            strInput = strInput.Trim();

            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                if (strInput.StartsWith("{{"))
                    return false;

                try
                {
                    JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }
    }
}
