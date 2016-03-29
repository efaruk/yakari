namespace Yakari
{
    public interface ICacheProviderOptions
    {
        ICacheCollectionObserver Observer { get; set; }

        int MaxRetryForLocalOperations { get; set; }

        int ConcurrencyLevel { get; set; }

        int InitialCapacity { get; set; }

        bool WaitForOthers { get; set; }
        
    }
}