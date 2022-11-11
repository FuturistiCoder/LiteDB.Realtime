using LiteDB.Realtime.Notifications;
using System;

namespace LiteDB.Realtime.Subscriptions
{
    internal class DocumentSubscription<T> : SubscriptionBase<T>
    {
        public BsonValue? Id { get; set; }

        public IObserver<T>? Observer { get; set; }

        public DocumentSubscription(NotificationService notificationService)
            : base(notificationService)
        { }

        public override void OnNextIfNeeded(NotificationCache cache)
            => _notificationService.NotifyIfNeeded(this, cache);
    }
}