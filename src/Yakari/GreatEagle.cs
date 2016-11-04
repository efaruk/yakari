using System;
using System.Threading.Tasks;

namespace Yakari
{
    public class GreatEagle : ICacheObserver
    {
        const string TempItemFormat = "yakari:{0}";

        readonly string _memberName;
        readonly ISubscriptionManager _subscriptionManager;
        readonly ILogger _logger;
        readonly ISerializer _serializer;
        readonly ILocalCacheProvider _localCacheProvider;
        readonly IRemoteCacheProvider _remoteCacheProvider;

        /// <summary>
        ///     Default constructor for GreatEagle
        /// </summary>
        /// <param name="memberName">Observing tribe member name </param>
        /// <param name="subscriptionManager"></param>
        /// <param name="serializer"></param>
        /// <param name="localCacheProvider"></param>
        /// <param name="remoteCacheProvider"></param>
        /// <param name="logger"></param>
        public GreatEagle(string memberName, ISubscriptionManager subscriptionManager, ISerializer serializer,
            ILocalCacheProvider localCacheProvider, IRemoteCacheProvider remoteCacheProvider, ILogger logger)
        {
            _memberName = memberName;
            _subscriptionManager = subscriptionManager;
            _logger = logger;
            _serializer = serializer;
            _localCacheProvider = localCacheProvider;
            _remoteCacheProvider = remoteCacheProvider;
            _subscriptionManager.OnMessageReceived += MessageSubscriberMessageReceived;
            StartObserving();
            LoadFromRemote();
        }

        private void LoadFromRemote()
        {
            ThreadHelper.RunOnDifferentThread(LoadFromRemoteInternal,
               ex => _logger.Log(LogLevel.Error, "Remote Cache Loading Error", ex));
            // try
            // {
            //     LoadFromRemoteInternal();
            // }
            // catch (Exception ex)
            // {
            //     _logger.Log(LogLevel.Error, "Remote Cache Loading Error", ex);
            // }
        }

        internal void LoadFromRemoteInternal()
        {
            var keys = _remoteCacheProvider.AllKeys();
            if (keys != null)
            {
                foreach (var key in keys)
                {
                    // TODO: Timeout should be configuration parameter
                    var item = _remoteCacheProvider.Get<InMemoryCacheItem>(key, TimeSpan.Zero, true);
                    _localCacheProvider.Set(key, item.ValueObject, item.ExpiresAtTimeSpan, true);
                }
            }
            _subscriptionManager.StartSubscription();
        }



        void StartObserving()
        {
            _localCacheProvider.OnBeforeGet += _localCacheProvider_OnBeforeGet;
            _localCacheProvider.OnAfterGet += _localCacheProvider_OnAfterGet;
            _localCacheProvider.OnBeforeSet += _localCacheProvider_OnBeforeSet;
            _localCacheProvider.OnAfterSet += _localCacheProvider_OnAfterSet;
            _localCacheProvider.OnBeforeDelete += _localCacheProvider_OnBeforeDelete;
            _localCacheProvider.OnAfterDelete += _localCacheProvider_OnAfterDelete;
        }

        void _localCacheProvider_OnAfterDelete(string key)
        {
            OnAfterDelete(key);
        }

        void _localCacheProvider_OnBeforeDelete(string key)
        {
            OnBeforeDelete(key);
        }

        void _localCacheProvider_OnAfterSet(string key)
        {
            OnAfterSet(key);
        }

        void _localCacheProvider_OnBeforeSet(string key, InMemoryCacheItem item)
        {
            OnBeforeSet(key, item);
        }

        void _localCacheProvider_OnAfterGet(string key)
        {
            OnAfterGet(key);
        }

        void _localCacheProvider_OnBeforeGet(string key, TimeSpan timeout)
        {
            OnBeforeGet(key, timeout);
        }

        void MessageSubscriberMessageReceived(string message)
        {
            _logger.Log(LogLevel.Trace, string.Format("GreatEagle Message Received: {0}", message));
            var cacheEvent = Deserialize(message);
            ThreadHelper.RunOnDifferentThread(() => Redirect(cacheEvent), true);
        }

        void Redirect(ICacheEventMessage message)
        {
            if (_memberName == message.MemberName) return;
            switch (message.CacheEventType)
            {
                case CacheEventType.Set:
                    OnRemoteSet(message.Key, message.MemberName);
                    break;
                case CacheEventType.Get:
                    break;
                case CacheEventType.Delete:
                    OnRemoteDelete(message.Key, message.MemberName);
                    break;
            }
        }

        // TODO: This method or some method should be triggered by developer(user) indirectly...
        public void OnBeforeSet(string key, InMemoryCacheItem item)
        {
            _logger.Log(LogLevel.Trace, string.Format("GreatEagle OnBeforeSet {0}", key));
            _remoteCacheProvider.Set(key, item, item.ExpiresAtTimeSpan, true);
            //TODO: Create temp remote cache item to make wait tribe for current member
        }

        public void OnAfterSet(string key)
        {
            _logger.Log(LogLevel.Trace, string.Format("GreatEagle OnAfterSet {0}", key));
            var data = new CacheEventMessage(key, _memberName, CacheEventType.Set);
            var message = Serialize(data);
            _subscriptionManager.Publish(message.ToString());
        }

        public void OnBeforeDelete(string key)
        {
            _logger.Log(LogLevel.Trace, string.Format("GreatEagle OnBeforeDelete {0}", key));
            //throw new NotImplementedException();
        }

        public void OnAfterDelete(string key)
        {
            _logger.Log(LogLevel.Trace, string.Format("GreatEagle OnAfterDelete {0}", key));
            _remoteCacheProvider.Delete(key, true);
            var data = new CacheEventMessage(key, _memberName, CacheEventType.Delete);
            var message = Serialize(data);
            _subscriptionManager.Publish(message.ToString());
        }

        public void OnBeforeGet(string key, TimeSpan timeout)
        {
            _logger.Log(LogLevel.Trace, string.Format("GreatEagle OnBeforeGet {0}", key));
            if (_localCacheProvider == null) return;
            if (_localCacheProvider.Exists(key)) return;
            var tempKey = string.Format(TempItemFormat, key);
            // Check key exists on master
            if (!_remoteCacheProvider.Exists(key))
            {
                // Key not exits
                // Check temp key exists on master
                if (!_remoteCacheProvider.Exists(tempKey)) return; // Temp key not exits
                // Temp key exists, so wait for tribe member to finish his job
                ThreadHelper.WaitFor(timeout);
            }
            //else
            //{
            //    // TODO: Investigate
            //    // Key exists
            //    // Check also temp key exists to be sure is ongoing update operation exists
            //    if (_remoteCacheProvider.Exists(tempKey))
            //    {
            //        // If so wait for update
            //        ThreadHelper.WaitFor(timeout);
            //    }
            //}
            var task = new Task<InMemoryCacheItem>(() =>
            {
                var it = _remoteCacheProvider.Get<InMemoryCacheItem>(key, timeout, true);
                return it;
            });
            task.Start();
            task.Wait(timeout);
            if (!task.IsCompleted) return;
            var item = task.Result;
            if (item == null) return;
            _localCacheProvider.Set(key, item.ValueObject, item.ExpiresAtTimeSpan, true);
        }

        public void OnAfterGet(string key)
        {
            _logger.Log(LogLevel.Trace, string.Format("GreatEagle OnAfterGet {0}", key));
            var cacheEventMessage = new CacheEventMessage(key, _memberName, CacheEventType.Get);
            var message = Serialize(cacheEventMessage);
            _subscriptionManager.Publish(message.ToString());
        }

        public void OnRemoteSet(string key, string memberName)
        {
            _logger.Log(LogLevel.Trace, string.Format("GreatEagle OnRemoteSet {0}", key));
            if (_memberName == memberName) return;
            if (_localCacheProvider == null) return;
            var item = _remoteCacheProvider.Get<InMemoryCacheItem>(key, TimeSpan.Zero, true);
            if (item == null) return;
            _localCacheProvider.Set(key, item.ValueObject, item.ExpiresAtTimeSpan, true);
        }

        public void OnRemoteDelete(string key, string memberName)
        {
            _logger.Log(LogLevel.Trace, string.Format("GreatEagle OnRemoteDelete {0}", key));
            if (_memberName == memberName) return;
            _localCacheProvider?.Delete(key, true);
        }

        public void Dispose()
        {
            _logger.Log(LogLevel.Trace, "GreatEagle Disposing");
            _subscriptionManager.StopSubscription();
            _subscriptionManager.OnMessageReceived -= MessageSubscriberMessageReceived;
        }

        object Serialize(CacheEventMessage cacheEvent)
        {
            return _serializer.Serialize(cacheEvent);
        }

        CacheEventMessage Deserialize(object message)
        {
            return _serializer.Deserialize<CacheEventMessage>(message);
        }
    }
}
