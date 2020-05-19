using FluentAssertions;
using LiteDB.Realtime.Subscriptions;
using Xunit;

namespace LiteDB.Realtime.Test.Subscriptions
{
    public class CollectionSubscriptionBuilder_Should : SubscriptionBuilderTestBase
    {
        private class Model
        { }

        public CollectionSubscriptionBuilder_Should()
            : base()
        {
        }

        [Fact]
        public void Build_A_Subscription()
        {
            var collectionName = "testCollection";
            new SubscriptionBuilder(_db.NotificationService)
                .Collection<Model>(collectionName)
                .Subscription
                .Collection
                .Should()
                .Be(collectionName);

            new SubscriptionBuilder(_db.NotificationService)
                .Collection<Model>(null)
                .Subscription
                .Collection
                .Should()
                .BeNull();
        }
    }
}