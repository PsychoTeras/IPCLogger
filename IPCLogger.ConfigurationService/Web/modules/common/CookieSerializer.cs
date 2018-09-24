using Nancy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace IPCLogger.ConfigurationService.Web.modules.common
{
    public class CookieSerializer : IObjectSerializer
    {
        public class TypeObject
        {
            public readonly string Type;
            public readonly object Value;

            public TypeObject(object value)
            {
                Value = value;
                Type = value?.GetType().AssemblyQualifiedName;
            }
        }

        public string Serialize(object sourceObject)
        {
            if (sourceObject == null)
            {
                return string.Empty;
            }

            string json = JsonConvert.SerializeObject(sourceObject as string ?? (object)new TypeObject(sourceObject));
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }

        public object Deserialize(string sourceString)
        {
            if (string.IsNullOrEmpty(sourceString))
            {
                return null;
            }

            byte[] inputBytes = Convert.FromBase64String(sourceString);
            string json = Encoding.UTF8.GetString(inputBytes);

            JToken jToken = JObject.Parse(json), jtType, jtValue;
            if ((jtType = jToken.SelectToken("Type")) == null || (jtValue = jToken.SelectToken("Value")) == null)
            {
                return JsonConvert.DeserializeObject(json);
            }

            Type objType = Type.GetType(jtType.ToString());
            return JsonConvert.DeserializeObject(jtValue.ToString(), objType); ;
        }
    }
}
