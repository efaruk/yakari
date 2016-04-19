namespace Yakari
{
    public class CacheEventMessage : ICacheEventMessage
    {

        public CacheEventMessage(string key, string memberName, CacheEventType cacheEventType)
        {
            Key = key;
            MemberName = memberName;
            CacheEventType = cacheEventType;
        }

        public string Key { get; }
        public string MemberName { get; }
        //public InMemoryCacheItem Data { get; }
        public CacheEventType CacheEventType { get; }

        public override bool Equals(object obj)
        {
            var cem = obj as CacheEventMessage;
            if (cem == null) return false;
            if (Key == cem.Key && MemberName == cem.MemberName && CacheEventType == cem.CacheEventType) return true;
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = Key.GetHashCode() ^ MemberName.GetHashCode() ^ CacheEventType.GetHashCode();
            return hashCode;
        }
    }
}