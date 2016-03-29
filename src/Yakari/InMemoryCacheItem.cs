using System;
using System.Threading;

namespace Yakari
{
    public class InMemoryCacheItem
    {
        private InMemoryCacheItem(bool slidable)
        {
            CreatedDateUtc = DateTime.UtcNow;
            Slidable = slidable;
        }

        public InMemoryCacheItem(object valueObject, DateTime expiresAt, bool slidable = false): this(slidable)
        {
            ValueObject = valueObject;
            ExpireDateUtc = expiresAt;
        }

        public InMemoryCacheItem(object valueObject, TimeSpan expiresAfter, bool slidable = false) : this(slidable)
        {
            ValueObject = valueObject;
            ExpireDateUtc = DateTime.UtcNow.Add(expiresAfter);
        }

        public bool Slidable { get; set; }

        /// <summary>
        ///     It has to be serializable with chosen serialization method.
        /// </summary>
        public object ValueObject { get; private set; }

        public DateTime ExpireDateUtc { get; private set; }

        public readonly DateTime CreatedDateUtc;

        private long _hitCount;
        public long HitCount
        {
            get
            {
                return Interlocked.Read(ref _hitCount);
            }
        }

        public override bool Equals(object obj)
        {
            var ici = obj as InMemoryCacheItem;
            if (ici == null) return false;
            return ValueObject.Equals(ici.ValueObject);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode : it has private setter
            return ValueObject.GetHashCode();
        }

        public void Hit()
        {
            Interlocked.Increment(ref _hitCount);
        }

        public void Slide(TimeSpan slideFor)
        {
            ExpireDateUtc = DateTime.UtcNow.Add(slideFor);
        }

        public bool IsExpired()
        {
            return DateTime.UtcNow >= ExpireDateUtc;
        }

        public bool WillBeExpired(TimeSpan after)
        {
            return DateTime.UtcNow.Add(after) >= ExpireDateUtc;
        }

        public bool WillBeExpired(DateTime at)
        {
            return at >= ExpireDateUtc;
        }

    }
}