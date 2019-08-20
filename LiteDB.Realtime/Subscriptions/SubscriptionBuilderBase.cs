using System;
using System.Collections.Generic;
using System.Text;

namespace LiteDB.Realtime.Subscriptions
{
    internal class SubscriptionBuilderBase<T> : ISubscriptionBuilderBase where T : class
    {
        protected readonly RealtimeLiteDatabase _database;
        protected readonly Subscription<T> _subscription;
        public SubscriptionBuilderBase(RealtimeLiteDatabase database, Subscription<T> subscription)
        {
            _database = database;
            _subscription = subscription;
        }

        ISubscription ISubscriptionBuilderBase.Subscription => _subscription;
    }
}
