using LiteDB.Realtime.Notifications;
using System;

namespace LiteDB.Realtime.Subscriptions
{
    internal abstract class SubscriptionBase<T> : ISubscription
    {
        protected readonly NotificationService _notificationService;
        public string Collection { get; set; } = string.Empty;
        public Type Type => typeof(T);

        private ILiteCollection<T>? _liteCollection;
        public ILiteCollection<T> LiteCollection
        { 
            get
            {
                if (_liteCollection is null)
                {
                    throw new InvalidOperationException("notification service is not initialized");
                }
                return _liteCollection;
            }
            set { _liteCollection = value; }
        }

        public SubscriptionBase(NotificationService notificationService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public abstract void OnNextIfNeeded(NotificationCache copy);
    }
}