namespace Yakari
{
    public class CacheEventMessage : ICacheEventMessage
    {
        public CacheEventMessage(string key, string memberName, InMemoryCacheItem data, CacheEventType cacheEventType)
        {
            Key = key;
            MemberName = memberName;
            Data = data;
            CacheEventType = cacheEventType;
        }

        public string Key { get; }
        public string MemberName { get; }
        public InMemoryCacheItem Data { get; }
        public CacheEventType CacheEventType { get; }
    }
}