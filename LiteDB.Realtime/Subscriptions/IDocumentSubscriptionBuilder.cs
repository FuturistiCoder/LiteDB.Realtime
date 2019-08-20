using System;
using System.Collections.Generic;

namespace LiteDB.Realtime.Subscriptions
{
    public interface IDocumentSubscriptionBuilder<T> : ISubscriptionBuilderBase, IObservable<List<T>> where T : class
    {
        IObservable<T> Id(BsonValue id);
    }
}