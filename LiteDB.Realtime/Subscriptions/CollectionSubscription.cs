using LiteDB.Realtime.Notifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LiteDB.Realtime.Subscriptions
{
    internal class CollectionSubscription<T> : SubscriptionBase<T> where T : class
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
