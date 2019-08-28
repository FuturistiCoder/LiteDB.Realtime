using System;
using System.Collections.Generic;

namespace LiteDB.Realtime.Subscriptions
{
    internal class DocumentSubscriptionBuilder<T> : SubscriptionBuilderBase<T>, IDocumentSubscriptionBuilder<T> where T : class
    {
        public DocumentSubscriptionBuilder(RealtimeLiteDatabase database, Subscription<T> subscription) : base(database, subscription)
        {
        }

        public IObservable<T> Id(BsonValue id)
        {
            _subscription.Id = id;
            return new SubscriptionBuilder<T>(_database, _subscription);
        }

        public IDisposable Subscribe(IObserver<List<T>> observer)
        {
            var builder = new SubscriptionBuilder<T>(_database, _subscription);
            return builder.Subscribe(observer);
        }
    }
}
