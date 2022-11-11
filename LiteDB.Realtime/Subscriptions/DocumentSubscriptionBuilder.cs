﻿using LiteDB.Realtime.Notifications;
using System;

namespace LiteDB.Realtime.Subscriptions
{
    internal class DocumentSubscriptionBuilder<T> : SubscriptionBuilderBase<T>, IObservable<T>
    {
        private readonly DocumentSubscription<T> _subscription;

        public DocumentSubscriptionBuilder(NotificationService notificationService, DocumentSubscription<T> subscription) : base(notificationService, subscription)
        {
            _subscription = subscription ?? throw new ArgumentNullException(nameof(subscription));
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