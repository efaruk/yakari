namespace Yakari
{
    public interface ILocalCacheProviderOptions
    {
//        ILogger Logger { get; set; }

        int MaxRetryForLocalOperations { get; set; }

        int ConcurrencyLevel { get; set; }

        int InitialCapacity { get; set; }

    }
}