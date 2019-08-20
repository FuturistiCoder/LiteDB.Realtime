using LiteDB.Engine;
using LiteDB.Realtime.Helpers;
using LiteDB.Realtime.Subscriptions;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

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
            _engine.NotificationCallback = OnNotified;
        }

        private void OnNotified(Notifications notifications)
        {
            if (notifications is null)
            {
                return;
            }

            foreach(var subscription in _subscriptions.Keys)
            {
                if (AreMatched(subscription, notifications))
                {
                    subscription.OnNext();
                }
            }
        }

        private bool AreMatched(ISubscription subscription, Notifications notifications)
        {
            foreach (var broadcast in notifications.Broadcasts)
            {
                if (broadcast.Key == subscription.Collection)
                {
                    return true;
                }
            }

            if (subscription.IsCollection)
            {
                foreach (var coll in notifications.Collections)
                {
                    // collection
                    if (coll.Key == subscription.Collection)
                    {
                        return true;
                    }
                }
            }
            else
            {
                foreach (var doc in notifications.Documents)
                {
                    (string coll, BsonValue id) = doc.Key;
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