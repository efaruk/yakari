using Newtonsoft.Json;

namespace Yakari.Serializers.Newtonsoft
{
    public class JsonNetSerializer : ISerializer
    {
        private JsonSerializerSettings _settings;

        public JsonNetSerializer()
        {
            _settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                NullValueHandling = NullValueHandling.Ignore,
            };
        }

        public JsonNetSerializer(JsonSerializerSettings settings)
        {
            _settings = settings;
        }

        public object Serialize<TInput>(TInput instance)
        {
            var serialized = JsonConvert.SerializeObject(instance, _settings);
            return serialized;
        }

        public TOutput Deserialize<TOutput>(object data)
        {
            var deserialized = JsonConvert.DeserializeObject<TOutput>((string)data, _settings);
            return deserialized;
        }
    }
}
