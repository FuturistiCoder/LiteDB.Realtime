using System;
using LiteDB.Realtime.Notifications;

namespace LiteDB.Realtime.Subscriptions
{
    internal class SubscriptionBuilder : ISubscriptionBuilder
    {
        private readonly NotificationService _notificationService;

        public SubscriptionBuilder(NotificationService notificationService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public ICollectionSubscriptionBuilder<T> Collection<T>(string collection) where T : class
        {
            var subscription = new CollectionSubscription<T>(_notificationService)
            {
                Collection = collection
            };
            return new CollectionSubscriptionBuilder<T>(_notificationService, subscription);
        }
    }
}
