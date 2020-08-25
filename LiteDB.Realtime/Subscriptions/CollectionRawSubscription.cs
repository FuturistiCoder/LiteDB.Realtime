using LiteDB.Realtime.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiteDB.Realtime.Subscriptions
{
    class CollectionRawSubscription<T> : SubscriptionBase<T> where T : class
    {
        public IObserver<ILiteCollection<T>>? Observer { get; set; }

        public CollectionRawSubscription(NotificationService notificationService)
            : base(notificationService)
        {
        }

        public override void OnNextIfNeeded(NotificationCache cache)
            => _notificationService.NotifyIfNeeded(this, cache);
    }
}
