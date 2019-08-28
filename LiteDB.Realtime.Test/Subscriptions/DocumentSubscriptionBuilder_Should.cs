using FluentAssertions;
using LiteDB.Realtime.Subscriptions;
using System;
using Xunit;

namespace LiteDB.Realtime.Test.Subscriptions
{
    public class DocumentSubscriptionBuilder_Should : SubscriptionBuilderTestBase
    {
        class Model { }

        [Fact]
        public void Build_A_Document_Subscription()
        {
            var collectionName = "testCollection";
            var id = new BsonValue(Guid.NewGuid());
            var sub = new Subscription<Model>(_db)
            {
                Collection = collectionName,
            };

            var builder = new DocumentSubscriptionBuilder<Model>(_db, sub);
            var observable = builder.Id(id);

            sub.Collection.Should().Be(collectionName);
            sub.Id.Should().Be(id);
            // before subscribing
            sub.ObserverObject.Should().BeNull();

            observable.Subscribe(obj => { });

            // after subscribing
            sub.IsCollection.Should().BeFalse();
            sub.ObserverObject.Should().NotBeNull();
            sub.AsCollectionObserver().Should().BeNull();
            sub.AsDocumentObserver().Should().NotBeNull();
        }

        [Fact]
        public void Build_A_Collection_Subscription()
        {
            var collectionName = "testCollection";
            var id = new BsonValue(Guid.NewGuid());
            var sub = new Subscription<Model>(_db)
            {
                Collection = collectionName,
            };

            var builder = new DocumentSubscriptionBuilder<Model>(_db, sub);
            sub.Collection.Should().Be(collectionName);
            sub.Id.Should().BeNull();
            // before subscribing
            sub.ObserverObject.Should().BeNull();

            builder.Subscribe(listObj => { });

            // after subscribing
            sub.IsCollection.Should().BeTrue();
            sub.ObserverObject.Should().NotBeNull();
            sub.AsCollectionObserver().Should().NotBeNull();
            sub.AsDocumentObserver().Should().BeNull();
        }
    }
}
