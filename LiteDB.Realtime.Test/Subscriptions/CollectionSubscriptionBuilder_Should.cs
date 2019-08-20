using FluentAssertions;
using LiteDB.Realtime.Subscriptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace LiteDB.Realtime.Test.Subscriptions
{
    public class CollectionSubscriptionBuilder_Should : SubscriptionBuilderTestBase
    {
        class Model { }

        public CollectionSubscriptionBuilder_Should()
            : base()
        {
        }

        [Fact]
        public void Build_A_Subscription()
        {
            var collectionName = "testCollection";
            new CollectionSubscriptionBuilder(_db)
                .Collection<Model>(collectionName)
                .Subscription
                .Collection
                .Should()
                .Be(collectionName);

            new CollectionSubscriptionBuilder(_db)
                .Collection<Model>(null)
                .Subscription
                .Collection
                .Should()
                .BeNull();
        }
    }
}
