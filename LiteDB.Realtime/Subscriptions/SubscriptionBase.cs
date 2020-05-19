using LiteDB.Realtime.Notifications;
using System;

namespace LiteDB.Realtime.Subscriptions
{
    internal abstract class SubscriptionBase<T> : ISubscription where T : class
    {
        protected readonly NotificationService _notificationService;
        public string Collection { get; set; } = string.Empty;
        public Type Type => typeof(T);
        // public object ObserverObject { get; set; }

        public SubscriptionBase(NotificationService notificationService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public abstract void OnNextIfNeeded(NotificationCache copy);
    }
}