using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace LiteDB.Realtime.Test.Notifications
{
    using Notifications = LiteDB.Realtime.Notifications;

    public class Notifications_Should
    {
        [Fact]
        public void Broadcast()
        {
            var notifications = new Notifications();
            notifications.Broadcasts.Count.Should().Be(0);

            notifications.BroadcastCollectionAndDocument("coll1");
            notifications.BroadcastCollectionAndDocument("coll2");

            notifications.Broadcasts.Count.Should().Be(2);
            notifications.Broadcasts.Contains("coll1").Should().BeTrue();
            notifications.Broadcasts.Contains("coll2").Should().BeTrue();
            notifications.Broadcasts.Contains("coll3").Should().BeFalse();

            notifications.Clear();
            notifications.Broadcasts.Count.Should().Be(0);
        }

        [Fact]
        public void Notify_Collection()
        {
            var notifications = new Notifications();
            notifications.Collections.Count.Should().Be(0);

            notifications.NotifyCollection("coll1");
            notifications.NotifyCollection("coll2");

            notifications.Collections.Count.Should().Be(2);
            notifications.Collections.Contains("coll1").Should().BeTrue();
            notifications.Collections.Contains("coll2").Should().BeTrue();
            notifications.Collections.Contains("coll3").Should().BeFalse();

            notifications.Clear();
            notifications.Collections.Count.Should().Be(0);
        }

        [Fact]
        public void Notify_Document()
        {
            var notifications = new Notifications();
            notifications.Documents.Count.Should().Be(0);

            notifications.NotifyDocument("coll1", new[] { new BsonValue(1), new BsonValue(2) });
            notifications.Documents.Count.Should().Be(2);

            notifications.NotifyDocument("coll2", new[] { new BsonValue(1), new BsonValue(3) });
            notifications.Documents.Count.Should().Be(4);

            notifications.Documents.Contains(("coll1", new BsonValue(1))).Should().BeTrue();
            notifications.Documents.Contains(("coll1", new BsonValue(2))).Should().BeTrue();
            notifications.Documents.Contains(("coll2", new BsonValue(1))).Should().BeTrue();
            notifications.Documents.Contains(("coll2", new BsonValue(3))).Should().BeTrue();

            notifications.Documents.Contains(("coll1", new BsonValue(3))).Should().BeFalse();
            notifications.Documents.Contains(("coll2", new BsonValue(2))).Should().BeFalse();

            notifications.Clear();
            notifications.Documents.Count.Should().Be(0);
        }

        [Fact]
        public void Not_Add_The_Document_Notification_If_It_Matches_With_Any_Broadcast()
        {
            var notifications = new Notifications();
            notifications.BroadcastCollectionAndDocument("coll1");
            notifications.BroadcastCollectionAndDocument("coll2");

            notifications.NotifyDocument("coll1", new[] { new BsonValue(1), new BsonValue(2) });
            notifications.Documents.Count.Should().Be(0);
        }

        [Fact]
        public void Add_The_Document_Notification_If_It_Does_Not_Match_With_Any_Broadcast()
        {
            var notifications = new Notifications();
            notifications.BroadcastCollectionAndDocument("coll1");
            notifications.BroadcastCollectionAndDocument("coll2");

            notifications.NotifyDocument("coll3", new[] { new BsonValue(1), new BsonValue(2) });
            notifications.Documents.Count.Should().Be(2);
        }


        [Fact]
        public void Not_Add_The_Collection_Notification_If_It_Matches_With_Any_Broadcast()
        {
            var notifications = new Notifications();
            notifications.BroadcastCollectionAndDocument("coll1");
            notifications.BroadcastCollectionAndDocument("coll2");

            notifications.NotifyCollection("coll1");
            notifications.Documents.Count.Should().Be(0);
        }

        [Fact]
        public void Add_The_Collection_Notification_If_It_Does_Not_Match_With_Any_Broadcast()
        {
            var notifications = new Notifications();
            notifications.BroadcastCollectionAndDocument("coll1");
            notifications.BroadcastCollectionAndDocument("coll2");

            notifications.NotifyCollection("coll3");
            notifications.Collections.Count.Should().Be(1);
        }

        [Fact]
        public void Remove_Matched_Document_Notification_If_A_Broadcast_Notification_Is_Added()
        {
            var notifications = new Notifications();
            notifications.NotifyDocument("coll1", new[] { new BsonValue(1), new BsonValue(2) });
            notifications.NotifyDocument("coll2", new[] { new BsonValue(1), new BsonValue(3) });
            notifications.Documents.Count.Should().Be(4);

            notifications.BroadcastCollectionAndDocument("coll1");
            notifications.Documents.Count.Should().Be(2);
            notifications.Documents.Where(doc => doc.Item1 == "coll2").Count().Should().Be(2);
        }

        [Fact]
        public void Remove_Matched_Collection_Notification_If_A_Broadcast_Notification_Is_Added()
        {
            var notifications = new Notifications();
            notifications.NotifyCollection("coll1");
            notifications.NotifyCollection("coll2");
            notifications.Collections.Count.Should().Be(2);

            notifications.BroadcastCollectionAndDocument("coll1");
            notifications.Collections.Count.Should().Be(1);
            notifications.Collections.Where(coll => coll == "coll2").Count().Should().Be(1);
        }
    }
}
