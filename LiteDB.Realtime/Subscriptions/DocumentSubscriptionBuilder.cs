using LiteDB.Realtime.Notifications;
using System;
using System.Collections.Generic;

namespace LiteDB.Realtime.Subscriptions
{
    internal class DocumentSubscriptionBuilder<T> : SubscriptionBuilderBase<T>, IObservable<T> where T : class
    {
        private readonly DocumentSubscription<T> _subscription;

        public DocumentSubscriptionBuilder(NotificationService notificationService, DocumentSubscription<T> subscription) : base(notificationService, subscription)
        {
            _subscription = subscription ?? throw new NullReferenceException(nameof(subscription));
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            _subscription.Observer = observer;
            var disposable = _notificationService.Subscribe(_subscription);
            _notificationService.Notify(_subscription);
            return disposable;
        }
    }
}
