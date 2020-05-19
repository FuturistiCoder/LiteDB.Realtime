using LiteDB.Realtime.Notifications;
using System;

namespace LiteDB.Realtime.Subscriptions
{
    internal class SubscriptionBuilderBase<T> : ISubscriptionBuilderBase where T : class
    {
        protected readonly NotificationService _notificationService;
        protected SubscriptionBase<T> _subscriptionBase;

        public SubscriptionBuilderBase(NotificationService notificationService, SubscriptionBase<T> subscription)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _subscriptionBase = subscription ?? throw new ArgumentNullException(nameof(subscription));
        }

        /// <summary>
        /// UnitTest Purpose
        /// </summary>
        ISubscription ISubscriptionBuilderBase.Subscription => _subscriptionBase;
    }
}