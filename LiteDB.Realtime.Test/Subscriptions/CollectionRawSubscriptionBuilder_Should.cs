using FluentAssertions;
using LiteDB.Realtime.Subscriptions;
using Xunit;
using System;

namespace LiteDB.Realtime.Test.Subscriptions
{
    public class CollectionRawSubscriptionBuilder_Should : SubscriptionBuilderTestBase
    {
        private class Model
        { }

        public CollectionRawSubscriptionBuilder_Should()
            : base()
        {
        }

        [Fact]
        public void Build_A_Raw_Subscription()
        {
            var collectionName = "testCollection";
            new SubscriptionBuilder(_db.NotificationService)
                .Collection<Model>(collectionName)
                .Raw
                .As<ISubscriptionBuilderBase>()
                .Subscription
                .Collection
                .Should()
                .Be(collectionName);

            new SubscriptionBuilder(_db.NotificationService)
                .Collection<Model>(null)
                .Raw
                .As<ISubscriptionBuilderBase>()
                .Subscription
                .Collection
                .Should()
                .BeNull();

            var sub = new SubscriptionBuilder(_db.NotificationService)
                .Collection<Model>(collectionName)
                .Raw
                .As<ISubscriptionBuilderBase>()
                .Subscription;
            sub.Collection.Should().Be(collectionName);

            var castedSub = sub.As<CollectionRawSubscription<Model>>();
            castedSub.Collection.Should().Be(collectionName);

            var builder = new CollectionRawSubscriptionBuilder<Model>(_db.NotificationService, castedSub);
            // before subscribing
            castedSub.Observer.Should().BeNull();

            builder.Subscribe(listObj => { });

            // after subscribing
            castedSub.Observer.Should().NotBeNull();
        }
    }
}