using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Yakari
{
    public class InMemoryCacheItem
    {
        private InMemoryCacheItem()
        {
        }

        private InMemoryCacheItem([AllowNull] TimeSpan? slideFor)
        {
            CreatedDateUtc = DateTime.UtcNow;
            SlideFor = slideFor;
        }

        public InMemoryCacheItem(object valueObject, DateTime expiresAt, [AllowNull] TimeSpan? slideFor = null) : this(slideFor)
        {
            ValueObject = valueObject;
            ExpireDateUtc = expiresAt;
        }

        public InMemoryCacheItem(object valueObject, TimeSpan expiresAfter, [AllowNull] TimeSpan? slideFor = null) : this(slideFor)
        {
            ValueObject = valueObject;
            ExpireDateUtc = CreatedDateUtc.Add(expiresAfter);
        }

        public TimeSpan? SlideFor { get; set; }

        /// <summary>
        ///     It has to be serializable with chosen serialization method.
        /// </summary>
        public object ValueObject { get; set; }

        public DateTime ExpireDateUtc { get; set; }

        public DateTime CreatedDateUtc { get; set; }

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
            return ValueObject == ici.ValueObject;
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode : it is immutable
            return ValueObject.GetHashCode();
        }

        public void Hit()
        {
            Interlocked.Increment(ref _hitCount);
            Slide();
        }

        private void Slide()
        {
            if (SlideFor.HasValue)
            {
                ExpireDateUtc = DateTime.UtcNow.Add(SlideFor.Value);
            }
        }

        public bool IsExpired
        {
            get
            {
                return DateTime.UtcNow > ExpireDateUtc;
            }
        }

        public bool WillBeExpired(TimeSpan after)
        {
            return DateTime.UtcNow.Add(after) > ExpireDateUtc;
        }

        public bool WillBeExpired(DateTime at)
        {
            return at > ExpireDateUtc;
        }

        public TimeSpan ExpiresAtTimeSpan
        {
            get
            {
                return ExpireDateUtc.Subtract(DateTime.UtcNow);
            }
        }
    }
}