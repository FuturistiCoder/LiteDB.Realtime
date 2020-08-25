using System;
using System.Collections.Generic;

namespace LiteDB.Realtime.Subscriptions
{
    public interface ICollectionSubscriptionBuilder<T> : ISubscriptionBuilderBase, IObservable<List<T>> where T : class
    {
        /// <summary>
        /// to make a document subscription
        /// </summary>
        /// <param name="id">document id</param>
        IObservable<T> Id(BsonValue id);

        /// <summary>
        /// to make a raw collection subscription
        /// this will return a `ILiteCollection<typeparamref name="T"/>` when the collection is updated.
        /// </summary>
        IObservable<ILiteCollection<T>> Raw { get; }
    }
}