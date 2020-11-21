using LiteDB.Realtime.Notifications;
using System;

namespace LiteDB.Realtime.Subscriptions
{
    internal class CollectionRawSubscriptionBuilder<T> : SubscriptionBuilderBase<T>, IObservable<ILiteCollection<T>> where T : class
    {
        private readonly CollectionRawSubscription<T> _subscription;

        public CollectionRawSubscriptionBuilder(NotificationService notification, CollectionRawSubscription<T> subscription)
            : base(notification, subscription)
        {
            _subscription = subscription ?? throw new ArgumentNullException(nameof(subscription));
        }

        public IDisposable Subscribe(IObserver<ILiteCollection<T>> observer)
        {
            _subscription.Observer = observer;
            var disposable = _notificationService.Subscribe(_subscription);
            _notificationService.Notify(_subscription);
            return disposable;
        }
    }
}