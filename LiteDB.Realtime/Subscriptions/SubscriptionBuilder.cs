using System;
using System.Collections.Generic;

namespace LiteDB.Realtime.Subscriptions
{
    internal class SubscriptionBuilder<T> : SubscriptionBuilderBase<T>, IObservable<T> where T : class
    {
        public SubscriptionBuilder(RealtimeLiteDatabase database, Subscription<T> subscription) : base(database, subscription)
        {
        }

        public IDisposable Subscribe(IObserver<List<T>> observer)
        {
            _subscription.IsCollection = true;
            _subscription.ObserverObject = observer;
            var disposable = _database.Subscribe(_subscription);
            _subscription.OnNext();
            return disposable;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            _subscription.ObserverObject = observer;
            var disposable = _database.Subscribe(_subscription);
            _subscription.OnNext();
            return disposable;
        }
    }
}
