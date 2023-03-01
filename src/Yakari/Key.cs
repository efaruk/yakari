namespace Yakari
{
    public class Key : IKey
    {
        public Key(string id, string region)
        {
            Id = id.NotNullOrWhitespace(nameof(id));
            Region = region;
        }

        public string Region { get; set; }

        public string Id { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Region))
            {
                return Id;
            }
            return $"{Region}:{Id}";
        }
    }
}