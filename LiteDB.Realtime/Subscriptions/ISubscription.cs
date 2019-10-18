using System;
using LiteDB.Realtime.Notifications;

namespace LiteDB.Realtime.Subscriptions
{
    internal interface ISubscription
    {
        string Collection { get; }
        Type Type { get; }
        void OnNextIfNeeded(NotificationCache copy);
    }
}
