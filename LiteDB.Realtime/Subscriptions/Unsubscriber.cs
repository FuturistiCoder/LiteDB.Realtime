using System;
using System.Collections.Concurrent;

namespace LiteDB.Realtime.Subscriptions
{
    using Subscriptions = ConcurrentDictionary<ISubscription, byte>;

    internal class Unsubscriber : IDisposable
    {
        private readonly Subscriptions _subscriptions;
        private readonly ISubscription _subscription;
        private bool _isDisposed = false;

        internal Unsubscriber(Subscriptions subscriptions, ISubscription subscription)
        {
            _subscriptions = subscriptions;
            _subscription = subscription;
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            if (_subscriptions.ContainsKey(_subscription))
            {
                _subscriptions.TryRemove(_subscription, out _);
            }

            _isDisposed = true;
            GC.SuppressFinalize(this);
        }

        ~Unsubscriber()
        {
            Dispose();
        }
    }
}