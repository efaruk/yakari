using System;
using System.Threading.Tasks;

namespace Yakari
{
    public class GreatEagle: ICacheManager
    {
        private const string TempItemFormat = "yakari:{0}";

        private readonly string _memberName;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IMessageSubscriber _messageSubscriber;
        private readonly ISerializer<string> _serializer;
        private ICacheProvider _localCacheProvider;
        private readonly ICacheProvider _remoteCacheProvider;

        public GreatEagle(string memberName, IMessagePublisher messagePublisher, IMessageSubscriber messageSubscriber, ISerializer<string> serializer, ICacheProvider remoteCacheProvider)
        {
            _memberName = memberName;
            _messagePublisher = messagePublisher;
            _messageSubscriber = messageSubscriber;
            _serializer = serializer;
            _remoteCacheProvider = remoteCacheProvider;
            _messageSubscriber.OnMessageReceived += MessageSubscriberMessageReceived;
        }

        private void MessageSubscriberMessageReceived(string message)
        {
            var cacheEvent = _serializer.Deserialize<CacheEventMessage>(message);
            ThreadHelper.RunOnDifferentThread(() => Redirect(cacheEvent), true);
        }

        private void Redirect(ICacheEventMessage message)
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

        public void SetupMember(ICacheProvider localCacheProvider)
        {
            _messageSubscriber.StartSubscription();
            _localCacheProvider = localCacheProvider;
        }

        // TODO: It should be triggered by develper(user) indirectly...
        public void OnBeforeSet(string key, InMemoryCacheItem item)
        {
            //TODO: Create temp remote cache item to make wait tribe for you
        }

        public void OnAfterSet(string key, InMemoryCacheItem item)
        {
            _remoteCacheProvider.Set(key, item, item.GetExpireTimeSpan());
            var data = new CacheEventMessage(key, _memberName, item, CacheEventType.Set);
            var message = _serializer.Serialize(data);
            _messagePublisher.Publish(message);
        }

        public void OnBeforeDelete(string key)
        {
            //throw new NotImplementedException();
        }

        public void OnAfterDelete(string key)
        {
            _remoteCacheProvider.Delete(key);
            var data = new CacheEventMessage(key, _memberName, null, CacheEventType.Delete);
            var message = _serializer.Serialize(data);
            _messagePublisher.Publish(message);
        }

        public void OnBeforeGet(string key, TimeSpan timeOut)
        {
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
                ThreadHelper.WaitFor(timeOut);
            }
            //else
            //{
            //    // TODO: Investigate
            //    // Key exists
            //    // Check also temp key exists to be sure is ongoing update operation exists
            //    if (_remoteCacheProvider.Exists(tempKey))
            //    {
            //        // If so wait for update
            //        ThreadHelper.WaitFor(timeOut);
            //    }
            //}
            var task = new Task<InMemoryCacheItem>(() =>
            {
                var it = _remoteCacheProvider.Get<InMemoryCacheItem>(key, timeOut);
                return it;
            });
            task.Start();
            task.Wait(timeOut);
            if (task.IsCompleted) return;
            var item = task.Result;
            if (item == null) return;
            _localCacheProvider.Set(key, item.ValueObject, item.GetExpireTimeSpan());
        }

        public void OnAfterGet(string key)
        {
            var data = new CacheEventMessage(key, _memberName, null, CacheEventType.Get);
            var message = _serializer.Serialize(data);
            _messagePublisher.Publish(message);
        }

        public void OnRemoteSet(string key, string memberName)
        {
            if (_memberName == memberName) return;
            if (_localCacheProvider == null) return;
            var item = _remoteCacheProvider.Get<InMemoryCacheItem>(key, TimeSpan.Zero);
            if (item == null) return;
            _localCacheProvider.Set(key, item.ValueObject, item.GetExpireTimeSpan());
        }

        public void OnRemoteDelete(string key, string memberName)
        {
            if (_memberName == memberName) return;
            if (_localCacheProvider == null) return;
            _localCacheProvider.Delete(key);
        }

        public void Dispose()
        {
            _messageSubscriber.StopSubscription();
            _messageSubscriber.OnMessageReceived -= MessageSubscriberMessageReceived;
        }
    }
}
