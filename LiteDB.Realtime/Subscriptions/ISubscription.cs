using System;
using System.Collections.Generic;
using System.Text;

namespace LiteDB.Realtime.Subscriptions
{
    internal interface ISubscription
    {
        string Collection { get; }
        BsonValue Id { get; }
        Type Type { get; }
        object ObserverObject { get; }
        bool IsCollection { get; }
        void OnNext();
    }
}
