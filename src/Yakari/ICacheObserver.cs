using System;

namespace Yakari
{
    public interface ICacheObserver
    {
        void BeforeSet(string key, InMemoryCacheItem item);

        void AfterSet(string key, InMemoryCacheItem item);

        void BeforeDelete(string key);

        void AfterDelete(string key);

        void BeforeGet(string key);

        void AfterGet(string key);

        //event CacheObservationEvent OnBeforeSet;

        //event CacheObservationEvent OnAfterSet;

        //event CacheObservationEvent OnBeforeDelete;

        //event CacheObservationEvent OnAfterDelete;

        //event CacheObservationEvent OnBeforeGet;

        //event CacheObservationEvent OnAfterGet;
    }

    //public delegate void CacheObservationEvent(ICacheObserver sender, CacheObsererEventArgs eventArgs);

    public class CacheObsererEventArgs: EventArgs
    {
        public CacheObsererEventArgs(string key)
        {
            Key = key;
        }

        public CacheObsererEventArgs(string key, InMemoryCacheItem item)
        {
            Key = key;
            Item = item;
        }

        public InMemoryCacheItem Item { get; }

        public string Key { get; }

    }
}