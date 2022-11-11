using LiteDB.Realtime.Notifications;
using System;

namespace LiteDB.Realtime.Subscriptions
{
    internal class SubscriptionBuilder : ISubscriptionBuilder
    {
        private readonly NotificationService _notificationService;

        public SubscriptionBuilder(NotificationService notificationService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public ICollectionSubscriptionBuilder<T> Collection<T>(string collection)
        {
            var subscription = new CollectionSubscription<T>(_notificationService)
            {
                Collection = collection
            };
            return new CollectionSubscriptionBuilder<T>(_notificationService, subscription);
        }
        public ICollectionSubscriptionBuilder<T> Collection<T>()
        {
            var defaultName = _notificationService.Database.GetCollection<T>().Name;
            var subscription = new CollectionSubscription<T>(_notificationService)
            {
                Collection = defaultName
            };
            return new CollectionSubscriptionBuilder<T>(_notificationService, subscription);
        }
    }
}