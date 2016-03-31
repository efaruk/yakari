namespace Yakari
{
    public interface ICacheEventMessage
    {
        string Key { get; }

        string MemberName { get; }

        InMemoryCacheItem Data { get; }

        CacheEventType CacheEventType { get; }
    }
}