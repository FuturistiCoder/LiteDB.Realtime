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
            cache.Broadcasts.Count.Should().Be(0);

            cache.AddBroadcast("coll1");
            cache.AddBroadcast("coll2");

            cache.Broadcasts.Count.Should().Be(2);
            cache.Broadcasts.Contains("coll1").Should().BeTrue();
            cache.Broadcasts.Contains("coll2").Should().BeTrue();
            cache.Broadcasts.Contains("coll3").Should().BeFalse();

            cache.Clear();
            cache.Broadcasts.Count.Should().Be(0);
        }

        [Fact]
        public void Add_Collection_Notification()
        {
            var cache = new NotificationCache();
            cache.Collections.Count.Should().Be(0);

            cache.AddCollection("coll1");
            cache.AddCollection("coll2");

            cache.Collections.Count.Should().Be(2);
            cache.Collections.Contains("coll1").Should().BeTrue();
            cache.Collections.Contains("coll2").Should().BeTrue();
            cache.Collections.Contains("coll3").Should().BeFalse();

            cache.Clear();
            cache.Collections.Count.Should().Be(0);
        }

        [Fact]
        public void Add_Document_Notification()
        {
            var cache = new NotificationCache();
            cache.Documents.Count.Should().Be(0);

            cache.AddDocuments("coll1", new[] { new BsonValue(1), new BsonValue(2) });
            cache.Documents.Count.Should().Be(2);

            cache.AddDocuments("coll2", new[] { new BsonValue(1), new BsonValue(3) });
            cache.Documents.Count.Should().Be(4);

            cache.Documents.Contains(("coll1", new BsonValue(1))).Should().BeTrue();
            cache.Documents.Contains(("coll1", new BsonValue(2))).Should().BeTrue();
            cache.Documents.Contains(("coll2", new BsonValue(1))).Should().BeTrue();
            cache.Documents.Contains(("coll2", new BsonValue(3))).Should().BeTrue();

            cache.Documents.Contains(("coll1", new BsonValue(3))).Should().BeFalse();
            cache.Documents.Contains(("coll2", new BsonValue(2))).Should().BeFalse();

            cache.Clear();
            cache.Documents.Count.Should().Be(0);
        }

        [Fact]
        public void Not_Add_The_Document_Notification_If_It_Matches_With_Any_Broadcast()
        {
            var cache = new NotificationCache();
            cache.AddBroadcast("coll1");
            cache.AddBroadcast("coll2");

            cache.AddDocuments("coll1", new[] { new BsonValue(1), new BsonValue(2) });
            cache.Documents.Count.Should().Be(0);
        }

        [Fact]
        public void Add_The_Document_Notification_If_It_Does_Not_Match_With_Any_Broadcast()
        {
            var cache = new NotificationCache();
            cache.AddBroadcast("coll1");
            cache.AddBroadcast("coll2");

            cache.AddDocuments("coll3", new[] { new BsonValue(1), new BsonValue(2) });
            cache.Documents.Count.Should().Be(2);
        }


        [Fact]
        public void Not_Add_The_Collection_Notification_If_It_Matches_With_Any_Broadcast()
        {
            var cache = new NotificationCache();
            cache.AddBroadcast("coll1");
            cache.AddBroadcast("coll2");

            cache.AddCollection("coll1");
            cache.Documents.Count.Should().Be(0);
        }

        [Fact]
        public void Add_The_Collection_Notification_If_It_Does_Not_Match_With_Any_Broadcast()
        {
            var cache = new NotificationCache();
            cache.AddBroadcast("coll1");
            cache.AddBroadcast("coll2");

            cache.AddCollection("coll3");
            cache.Collections.Count.Should().Be(1);
        }

        [Fact]
        public void Remove_Matched_Document_Notification_If_A_Broadcast_Notification_Is_Added()
        {
            var cache = new NotificationCache();
            cache.AddDocuments("coll1", new[] { new BsonValue(1), new BsonValue(2) });
            cache.AddDocuments("coll2", new[] { new BsonValue(1), new BsonValue(3) });
            cache.Documents.Count.Should().Be(4);

            cache.AddBroadcast("coll1");
            cache.Documents.Count.Should().Be(2);
            cache.Documents.Where(doc => doc.Item1 == "coll2").Count().Should().Be(2);
        }

        [Fact]
        public void Remove_Matched_Collection_Notification_If_A_Broadcast_Notification_Is_Added()
        {
            var cache = new NotificationCache();
            cache.AddCollection("coll1");
            cache.AddCollection("coll2");
            cache.Collections.Count.Should().Be(2);

            cache.AddBroadcast("coll1");
            cache.Collections.Count.Should().Be(1);
            cache.Collections.Where(coll => coll == "coll2").Count().Should().Be(1);
        }
    }
}
