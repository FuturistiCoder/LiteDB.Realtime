using LiteDB.Realtime.Notifications;
using System;

namespace LiteDB.Realtime.Subscriptions
{
    internal interface ISubscription
    {
        string Collection { get; }
        Type Type { get; }

        void OnNextIfNeeded(NotificationCache copy);
    }
}