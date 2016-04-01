using Newtonsoft.Json;

namespace Yakari.Serializers.Newtonsoft
{
    public class JsonSerializer: ISerializer<string>
    {
        public JsonSerializer()
        {
            
        }

        public string Serialize<TInput>(TInput instance)
        {
            var serialized = JsonConvert.SerializeObject(instance);
            return serialized;
        }

        public TOutput Deserialize<TOutput>(string data)
        {
            var deserialized = JsonConvert.DeserializeObject<TOutput>(data);
            return deserialized;
        }
    }
}
