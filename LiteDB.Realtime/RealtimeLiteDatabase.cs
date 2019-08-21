using LiteDB.Engine;
using LiteDB.Realtime.Helpers;
using LiteDB.Realtime.Notifications;
using LiteDB.Realtime.Subscriptions;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace LiteDB.Realtime
{
    using SubscriptionDict = ConcurrentDictionary<ISubscription, byte>;

    public class RealtimeLiteDatabase : LiteDatabase
    {
        private readonly RealtimeLiteEngine _engine;
        private readonly SubscriptionDict _subscriptions = new SubscriptionDict();

        public RealtimeLiteDatabase(string connectionString, BsonMapper mapper = null)
            : this(new ConnectionString(connectionString), mapper)
        {
        }

        public RealtimeLiteDatabase(ConnectionString connectionString, BsonMapper mapper = null)
            : this(connectionString.CreateRealtimeEngine(), mapper)
        {
        }

        public RealtimeLiteDatabase(Stream stream, BsonMapper mapper = null)
            : this(stream.CreateRealtimeEngine(), mapper)
        {
        }

        public RealtimeLiteDatabase(ILiteEngine engine, BsonMapper mapper = null)
            : base(engine, mapper)
        {
            _engine = engine as RealtimeLiteEngine;
            if (_engine is null)
            {
                throw new NotSupportedException();
            }
            _engine._notificationService.NotificationCallback = OnNotified;
        }

        private void OnNotified(NotificationCache cache)
        {
            if (cache is null)
            {
                return;
            }

            foreach (var subscription in _subscriptions.Keys)
            {
                if (AreMatched(subscription, cache))
                {
                    subscription.OnNext();
                }
            }
        }

        private bool AreMatched(ISubscription subscription, NotificationCache cache)
        {
            foreach (var broadcast in cache.Broadcasts)
            {
                if (broadcast == subscription.Collection)
                {
                    return true;
                }
            }

            if (subscription.IsCollection)
            {
                foreach (var coll in cache.Collections)
                {
                    // collection
                    if (coll == subscription.Collection)
                    {
                        return true;
                    }
                }
            }
            else
            {
                foreach (var doc in cache.Documents)
                {
                    (string coll, BsonValue id) = doc;
                    if (coll == subscription.Collection && id == subscription.Id)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal IDisposable Subscribe(ISubscription subscription)
        {
            _subscriptions.TryAdd(subscription, default);
            return new Unsubscriber(_subscriptions, subscription);
        }

        public ICollectionSubscriptionBuilder Realtime => new CollectionSubscriptionBuilder(this);
    }
}