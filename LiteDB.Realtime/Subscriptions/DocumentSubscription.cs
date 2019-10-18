using LiteDB.Realtime.Notifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LiteDB.Realtime.Subscriptions
{
    internal class DocumentSubscription<T> : SubscriptionBase<T> where T : class
    {
        public BsonValue? Id { get; set; }
        // public IObserver<T> AsObserver() => ObserverObject as IObserver<T>;
        public IObserver<T>? Observer { get; set; }

        public DocumentSubscription(NotificationService notificationService)
            : base(notificationService)
        {}


        public override void OnNextIfNeeded(NotificationCache cache)
            => _notificationService.NotifyIfNeeded(this, cache);
    }
}
