﻿using LiteDB.Realtime.Notifications;
using System;
using System.Collections.Generic;

namespace LiteDB.Realtime.Subscriptions
{
    internal class CollectionSubscription<T> : SubscriptionBase<T>
    {
        public IObserver<List<T>>? Observer { get; set; }

        public CollectionSubscription(NotificationService notificationService)
            : base(notificationService)
        {
        }

        public override void OnNextIfNeeded(NotificationCache cache)
            => _notificationService.NotifyIfNeeded(this, cache);
    }
}