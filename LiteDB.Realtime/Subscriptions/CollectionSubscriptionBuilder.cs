using LiteDB.Realtime.Notifications;
using System;
using System.Collections.Generic;

namespace LiteDB.Realtime.Subscriptions
{
    internal class CollectionSubscriptionBuilder<T> : SubscriptionBuilderBase<T>, ICollectionSubscriptionBuilder<T> where T : class
    {
        private readonly CollectionSubscription<T> _subscription;

        public CollectionSubscriptionBuilder(NotificationService notificationService, CollectionSubscription<T> subscription)
            : base(notificationService, subscription)
        {
            _subscription = subscription ?? throw new NullReferenceException(nameof(subscription));
        }

        public IObservable<T> Id(BsonValue id)
        {
            var documentSubscription = new DocumentSubscription<T>(_notificationService)
            {
                Id = id,
                Collection = _subscription.Collection
            };
            return new DocumentSubscriptionBuilder<T>(_notificationService, documentSubscription);
        }

        public IObservable<ILiteCollection<T>> Raw
        { 
            get
            {
                var collectionRawSubscription = new CollectionRawSubscription<T>(_notificationService)
                {
                    Collection = _subscription.Collection
                };
                return new CollectionRawSubscriptionBuilder<T>(_notificationService, collectionRawSubscription);
            }
        }

        public IDisposable Subscribe(IObserver<List<T>> observer)
        {
            _subscription.Observer = observer;
            var disposable = _notificationService.Subscribe(_subscription);
            _notificationService.Notify(_subscription);
            return disposable;
        }
    }
}