using System;
using Yakari.Interfaces;

namespace Yakari
{
    public class LocalCacheProviderOptions : ILocalCacheProviderOptions
    {
        const int MinimumMaxRetryForLocalOperations = 1;
        const int DefaultMaxRetryForLocalOperations = 1000;
        const int MinimumConcurrencyLevel = 2;
        const int DefaultConcurrencyLevel = 10;
        const int MinimumInitialCapacity = 10;
        const int DefaultInitialCapacity = 100;

        public LocalCacheProviderOptions(ILogger logger, int maxRetryForLocalOperations = DefaultMaxRetryForLocalOperations, int concurrencyLevel = DefaultConcurrencyLevel, int initialCapacity = DefaultInitialCapacity, bool dontWaitForTribe = false)
        {
            // Check
            if (logger == null) throw new ArgumentNullException("logger");
            if (maxRetryForLocalOperations < MinimumMaxRetryForLocalOperations) throw new ArgumentOutOfRangeException("maxRetryForLocalOperations");
            // Set
            Logger = logger;
            MaxRetryForLocalOperations = maxRetryForLocalOperations;
            ConcurrencyLevel = concurrencyLevel;
            InitialCapacity = initialCapacity;
            DontWaitForTribe = dontWaitForTribe;
            // Defaults
            if (ConcurrencyLevel == 0) ConcurrencyLevel = DefaultConcurrencyLevel;
            if (ConcurrencyLevel < MinimumConcurrencyLevel) ConcurrencyLevel = MinimumConcurrencyLevel;
            if (InitialCapacity == 0) InitialCapacity = DefaultInitialCapacity;
            if (InitialCapacity < MinimumInitialCapacity) InitialCapacity = MinimumInitialCapacity;
        }

        public ILogger Logger { get; set; }
        public int MaxRetryForLocalOperations { get; set; }
        public int ConcurrencyLevel { get; set; }
        public int InitialCapacity { get; set; }
        public bool DontWaitForTribe { get; set; }
    }
}