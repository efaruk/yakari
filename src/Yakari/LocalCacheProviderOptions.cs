using System;

namespace Yakari
{
    public class LocalCacheProviderOptions
    {
        private const int MinimumMaxRetryForLocalOperations = 1;
        private const int DefaultMaxRetryForLocalOperations = 1000;
        private const int MinimumConcurrencyLevel = 2;
        private const int DefaultConcurrencyLevel = 10;
        private const int MinimumInitialCapacity = 10;
        private const int DefaultInitialCapacity = 100;

        public LocalCacheProviderOptions()
        {
            //if (logger == null) throw new ArgumentNullException("logger");
            //Logger = logger;
            // Defaults
            MaxRetryForLocalOperations = DefaultMaxRetryForLocalOperations;
            ConcurrencyLevel = DefaultConcurrencyLevel;
            InitialCapacity = DefaultInitialCapacity;
        }

        public LocalCacheProviderOptions(int maxRetryForLocalOperations = DefaultMaxRetryForLocalOperations, int concurrencyLevel = DefaultConcurrencyLevel, int initialCapacity = DefaultInitialCapacity) : this()
        {
            // Check
            if (maxRetryForLocalOperations < MinimumMaxRetryForLocalOperations) throw new ArgumentOutOfRangeException("maxRetryForLocalOperations");
            // Set
            MaxRetryForLocalOperations = maxRetryForLocalOperations;
            ConcurrencyLevel = concurrencyLevel;
            InitialCapacity = initialCapacity;

            if (ConcurrencyLevel < MinimumConcurrencyLevel) ConcurrencyLevel = MinimumConcurrencyLevel;
            if (InitialCapacity < MinimumInitialCapacity) InitialCapacity = MinimumInitialCapacity;
            if (MaxRetryForLocalOperations < MinimumMaxRetryForLocalOperations)
                MaxRetryForLocalOperations = DefaultMaxRetryForLocalOperations;
        }

        //public ILogger Logger { get; set; }
        public int MaxRetryForLocalOperations { get; set; }

        public int ConcurrencyLevel { get; set; }
        public int InitialCapacity { get; set; }
    }
}