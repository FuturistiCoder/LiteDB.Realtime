using FluentAssertions;
using LiteDB.Realtime.Notifications;
using System.Linq;
using Xunit;

namespace LiteDB.Realtime.Test.Notifications
{
    public class NotificationCache_Should
    {
        [Fact]
        public void Add_Broadcast_Notification()
        {
            var cache = new NotificationCache();
            cache.Broadcasts.Should().BeEmpty();

            cache.AddBroadcast("coll1");
            cache.AddBroadcast("coll2");

            cache.Broadcasts.Should().HaveCount(2);
            cache.Broadcasts.Contains("coll1").Should().BeTrue();
            cache.Broadcasts.Contains("coll2").Should().BeTrue();
            cache.Broadcasts.Contains("coll3").Should().BeFalse();

            cache.Clear();
            cache.Broadcasts.Should().BeEmpty();
        }

        [Fact]
        public void Add_Collection_Notification()
        {
            var cache = new NotificationCache();
            cache.Collections.Should().BeEmpty();

            cache.AddCollection("coll1");
            cache.AddCollection("coll2");

            cache.Collections.Should().HaveCount(2);
            cache.Collections.Contains("coll1").Should().BeTrue();
            cache.Collections.Contains("coll2").Should().BeTrue();
            cache.Collections.Contains("coll3").Should().BeFalse();

            cache.Clear();
            cache.Collections.Should().BeEmpty();
        }

        [Fact]
        public void Add_Document_Notification()
        {
            var cache = new NotificationCache();
            cache.Documents.Should().BeEmpty();

            cache.AddDocuments("coll1", new[] { new BsonValue(1), new BsonValue(2) });
            cache.Documents.Should().HaveCount(2);

            cache.AddDocuments("coll2", new[] { new BsonValue(1), new BsonValue(3) });
            cache.Documents.Should().HaveCount(4);

            cache.Documents.Contains(("coll1", new BsonValue(1))).Should().BeTrue();
            cache.Documents.Contains(("coll1", new BsonValue(2))).Should().BeTrue();
            cache.Documents.Contains(("coll2", new BsonValue(1))).Should().BeTrue();
            cache.Documents.Contains(("coll2", new BsonValue(3))).Should().BeTrue();

            cache.Documents.Contains(("coll1", new BsonValue(3))).Should().BeFalse();
            cache.Documents.Contains(("coll2", new BsonValue(2))).Should().BeFalse();

            cache.Clear();
            cache.Documents.Should().BeEmpty();
        }

        [Fact]
        public void Add_Collection_Notification_When_Adding_Document_Notification()
        { 
            var cache = new NotificationCache();
            cache.Documents.Should().BeEmpty();

            cache.AddDocuments("coll1", new[] { new BsonValue(1), new BsonValue(2) });
            cache.Documents.Should().HaveCount(2);

            cache.Collections.Contains("coll1").Should().BeTrue();
        }

        [Fact]
        public void Not_Add_The_Document_Notification_If_It_Matches_With_Any_Broadcast()
        {
            var cache = new NotificationCache();
            cache.AddBroadcast("coll1");
            cache.AddBroadcast("coll2");

            cache.AddDocuments("coll1", new[] { new BsonValue(1), new BsonValue(2) });
            cache.Documents.Should().BeEmpty();
        }

        [Fact]
        public void Add_The_Document_Notification_If_It_Does_Not_Match_With_Any_Broadcast()
        {
            var cache = new NotificationCache();
            cache.AddBroadcast("coll1");
            cache.AddBroadcast("coll2");

            cache.AddDocuments("coll3", new[] { new BsonValue(1), new BsonValue(2) });
            cache.Documents.Should().HaveCount(2);
        }


        [Fact]
        public void Not_Add_The_Collection_Notification_If_It_Matches_With_Any_Broadcast()
        {
            var cache = new NotificationCache();
            cache.AddBroadcast("coll1");
            cache.AddBroadcast("coll2");

            cache.AddCollection("coll1");
            cache.Documents.Should().BeEmpty();
        }

        [Fact]
        public void Add_The_Collection_Notification_If_It_Does_Not_Match_With_Any_Broadcast()
        {
            var cache = new NotificationCache();
            cache.AddBroadcast("coll1");
            cache.AddBroadcast("coll2");

            cache.AddCollection("coll3");
            cache.Collections.Should().HaveCount(1);
        }

        [Fact]
        public void Remove_Matched_Document_Notification_If_A_Broadcast_Notification_Is_Added()
        {
            var cache = new NotificationCache();
            cache.AddDocuments("coll1", new[] { new BsonValue(1), new BsonValue(2) });
            cache.AddDocuments("coll2", new[] { new BsonValue(1), new BsonValue(3) });
            cache.Documents.Should().HaveCount(4);

            cache.AddBroadcast("coll1");
            cache.Documents.Should().HaveCount(2);
            cache.Documents.Where(doc => doc.Item1 == "coll2").Count().Should().Be(2);
        }

        [Fact]
        public void Remove_Matched_Collection_Notification_If_A_Broadcast_Notification_Is_Added()
        {
            var cache = new NotificationCache();
            cache.AddCollection("coll1");
            cache.AddCollection("coll2");
            cache.Collections.Should().HaveCount(2);

            cache.AddBroadcast("coll1");
            cache.Collections.Should().HaveCount(1);
            cache.Collections.Where(coll => coll == "coll2").Count().Should().Be(1);
        }
    }
}
