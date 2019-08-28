using FluentAssertions;
using LiteDB.Realtime.Subscriptions;
using System.Collections.Concurrent;
using Xunit;

namespace LiteDB.Realtime.Test.Subscriptions
{
    using SubscriptionDict = ConcurrentDictionary<ISubscription, byte>;
    public class Unsubscriber_Should : SubscriptionBuilderTestBase
    {
        class Model { }

        [Fact]
        public void Unsubscribe_From_DB_When_Disposing()
        {
            var subscriptions = new SubscriptionDict();
            var sub1 = new Subscription<Model>(_db);
            var sub2 = new Subscription<Model>(_db);
            subscriptions.TryAdd(sub1, default);
            subscriptions.TryAdd(sub2, default);
            subscriptions.Keys.Should().HaveCount(2);
            subscriptions.Keys.Should().Contain(sub1);
            subscriptions.Keys.Should().Contain(sub2);

            using (var unsubscriber = new Unsubscriber(subscriptions, sub1))
            { }

            subscriptions.Keys.Should().HaveCount(1);
            subscriptions.Keys.Should().Contain(sub2);
        }
    }
}
