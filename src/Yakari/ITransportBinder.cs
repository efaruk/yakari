namespace Yakari
{
    public interface ITransportBinder
    {

    }

    public interface ICacheEventArgs
    {
        string Key { get; }

        object Data { get; }

        CacheEventType CacheEventType { get; }
    }

    public class CacheEventArgs : ICacheEventArgs
    {
        public string Key { get; }
        public object Data { get; }
        public CacheEventType CacheEventType { get; }
    }

    public enum CacheEventType
    {
        Get,
        Set,
        Delete
    }
}