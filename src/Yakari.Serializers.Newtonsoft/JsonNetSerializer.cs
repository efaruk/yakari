using Newtonsoft.Json;

namespace Yakari.Serializers.Newtonsoft
{
    public class JsonNetSerializer: ISerializer
    {
        public object Serialize<TInput>(TInput instance)
        {
            var serialized = JsonConvert.SerializeObject(instance);
            return serialized;
        }

        public TOutput Deserialize<TOutput>(object data)
        {
            var deserialized = JsonConvert.DeserializeObject<TOutput>((string)data);
            return deserialized;
        }
    }
}
