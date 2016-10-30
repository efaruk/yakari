using System;

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

        public LocalCacheProviderOptions(ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            Logger = logger;
            // Defaults
            MaxRetryForLocalOperations = DefaultMaxRetryForLocalOperations;
            ConcurrencyLevel = DefaultConcurrencyLevel;
            InitialCapacity = DefaultInitialCapacity;
        }

        public LocalCacheProviderOptions(ILogger logger, int maxRetryForLocalOperations = DefaultMaxRetryForLocalOperations, int concurrencyLevel = DefaultConcurrencyLevel, int initialCapacity = DefaultInitialCapacity, bool dontWaitForTribe = false): this(logger)
        {
            // Check
            if (maxRetryForLocalOperations < MinimumMaxRetryForLocalOperations) throw new ArgumentOutOfRangeException("maxRetryForLocalOperations");
            // Set
            MaxRetryForLocalOperations = maxRetryForLocalOperations;
            ConcurrencyLevel = concurrencyLevel;
            InitialCapacity = initialCapacity;
            DontWaitForTribe = dontWaitForTribe;

            if (ConcurrencyLevel < MinimumConcurrencyLevel) ConcurrencyLevel = MinimumConcurrencyLevel;
            if (InitialCapacity < MinimumInitialCapacity) InitialCapacity = MinimumInitialCapacity;
            if (MaxRetryForLocalOperations < MinimumMaxRetryForLocalOperations)
                MaxRetryForLocalOperations = DefaultMaxRetryForLocalOperations;
        }

        public ILogger Logger { get; set; }
        public int MaxRetryForLocalOperations { get; set; }
        public int ConcurrencyLevel { get; set; }
        public int InitialCapacity { get; set; }
        public bool DontWaitForTribe { get; set; }
    }
}