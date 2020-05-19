using LiteDB.Realtime.Subscriptions;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace LiteDB.Realtime.Notifications
{
    using Subscriptions = ConcurrentDictionary<ISubscription, byte>;

    internal class NotificationService
    {
        public NotificationCache Cache { get; private set; } = new NotificationCache();

        private Subscriptions _subscriptions = new Subscriptions();
        private LiteDatabase? _database;

        public void Init(LiteDatabase database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public SubscriptionBuilder SubscriptionBuilder() => new SubscriptionBuilder(this);

        internal void NotifyIfNeeded<T>(CollectionSubscription<T> collectionSubscription, NotificationCache cache) where T : class
        {
            if (cache.Broadcasts.Contains(collectionSubscription.Collection))
            {
                Notify(collectionSubscription);
                return;
            }

            if (cache.Collections.Contains(collectionSubscription.Collection))
            {
                Notify(collectionSubscription);
                return;
            }
        }

        internal void NotifyIfNeeded<T>(DocumentSubscription<T> documentSubscription, NotificationCache cache) where T : class
        {
            if (cache.Broadcasts.Contains(documentSubscription.Collection))
            {
                Notify(documentSubscription);
                return;
            }

            if (documentSubscription.Id is null)
            {
                return;
            }

            if (cache.Documents.Contains((documentSubscription.Collection, documentSubscription.Id)))
            {
                Notify(documentSubscription);
                return;
            }
        }

        internal IDisposable Subscribe<T>(SubscriptionBase<T> subscription) where T : class
        {
            _subscriptions.TryAdd(subscription, default);
            return new Unsubscriber(_subscriptions, subscription);
        }

        /// <summary>
        /// Notify all matched subscriptions and clean cache
        /// </summary>
        public void Notify()
        {
            var copy = Cache;
            Cache = new NotificationCache();

            foreach (var subscription in _subscriptions.Keys)
            {
                subscription.OnNextIfNeeded(copy);
            }
        }

        /// <summary>
        /// Notify one subscription
        /// </summary>
        public void Notify<T>(CollectionSubscription<T> collectionSubscription) where T : class
        {
            if (_database is null)
            {
                throw new InvalidOperationException("notification service is not initialized");
            }

            var nextValue = _database.GetCollection<T>(collectionSubscription.Collection).Query().ToList();
            Task.Run(() => collectionSubscription.Observer?.OnNext(nextValue));
        }

        /// <summary>
        /// Notify one subscription
        /// </summary>
        public void Notify<T>(DocumentSubscription<T> documentSubscription) where T : class
        {
            if (_database is null)
            {
                throw new InvalidOperationException("notification service is not initialized");
            }

            var nextValue = _database.GetCollection<T>(documentSubscription.Collection).FindById(documentSubscription.Id);
            Task.Run(() => documentSubscription.Observer?.OnNext(nextValue));
        }

        /// <summary>
        /// Clear notification cache
        /// </summary>
        public void Clear()
        {
            Cache.Clear();
        }
    }
}