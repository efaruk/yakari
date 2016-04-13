namespace Yakari
{
    public interface ILocalCacheProviderOptions
    {
        ILogger Logger { get; set; }

        //ICacheManager Manager { get; set; }

        int MaxRetryForLocalOperations { get; set; }

        int ConcurrencyLevel { get; set; }

        int InitialCapacity { get; set; }

        bool DontWaitForTribe { get; set; }

    }
}