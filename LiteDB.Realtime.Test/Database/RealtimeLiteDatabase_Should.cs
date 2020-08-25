using FluentAssertions;
using LiteDB.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Xunit;

namespace LiteDB.Realtime.Test.Database
{
    public class RealtimeLiteDatabase_Should
    {
        private class Item
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
        }

        [Fact]
        public void Be_Created_By_Constructors()
        {
            var fileName = Path.GetTempPath() + Path.GetRandomFileName();
            {
                using var db1 = new RealtimeLiteDatabase(fileName);
            }

            {
                using var db2 = new RealtimeLiteDatabase(new ConnectionString
                {
                    Filename = fileName
                });
            }

            {
                using var db3 = new RealtimeLiteDatabase(new MemoryStream { });
            }

            {
                using var db4 = new RealtimeLiteDatabase(new RealtimeLiteEngine(new LiteEngine()));
            }
        }

        [Fact]
        public void Notify_Collection_Subscription_When_A_New_Data_Added()
        {
            using var db = new RealtimeLiteDatabase(new MemoryStream());
            List<Item> receivedItems = null;
            int rawOnNextCount = 0;
            // collection subscription
            using var colSub = db.Realtime.Collection<Item>("items").Subscribe(items => receivedItems = items);
            // collection raw subscription
            using var rawColSub = db.Realtime.Collection<Item>("items").Raw.Subscribe(liteCollection => rawOnNextCount++);

            //waiting for notification
            Thread.Sleep(TimeSpan.FromSeconds(1));
            receivedItems.Should().BeEmpty();
            rawOnNextCount.Should().Be(1);

            var newItem = new Item
            {
                Name = "Keyboard",
                Price = 100m
            };
            var newId = db.GetCollection<Item>("items").Insert(newItem);
            newId.IsGuid.Should().BeTrue();

            // waiting for notification
            Thread.Sleep(TimeSpan.FromSeconds(1));

            receivedItems.Should().NotBeNull();
            receivedItems.Should().HaveCount(1);
            receivedItems[0].Id.Should().Be(newId.AsGuid);
            receivedItems[0].Name.Should().Be(newItem.Name);
            receivedItems[0].Price.Should().Be(newItem.Price);

            rawOnNextCount.Should().Be(2);
        }

        [Fact]
        public void Notify_Collection_Subscription_When_A_Data_Upserted()
        {
            using var db = new RealtimeLiteDatabase(new MemoryStream());
            List<Item> receivedItems = null;
            int rawOnNextCount = 0;
            // collection subscription
            using var colSub = db.Realtime.Collection<Item>("items").Subscribe(items => receivedItems = items);
            // collection raw subscription
            using var rawColSub = db.Realtime.Collection<Item>("items").Raw.Subscribe(liteCollection => rawOnNextCount++);

            //waiting for notification
            Thread.Sleep(TimeSpan.FromSeconds(1));
            receivedItems.Should().BeEmpty();
            rawOnNextCount.Should().Be(1);

            var newItem = new Item
            {
                Id = Guid.NewGuid(),
                Name = "Keyboard",
                Price = 100m
            };
            bool isInsert = db.GetCollection<Item>("items").Upsert(newItem);
            isInsert.Should().BeTrue();

            // waiting for notification
            Thread.Sleep(TimeSpan.FromSeconds(1));

            receivedItems.Should().NotBeNull();
            receivedItems.Should().HaveCount(1);
            receivedItems[0].Id.Should().Be(newItem.Id);
            receivedItems[0].Name.Should().Be(newItem.Name);
            receivedItems[0].Price.Should().Be(newItem.Price);
            rawOnNextCount.Should().Be(2);

            // update item
            newItem.Price = 99m;
            isInsert = db.GetCollection<Item>("items").Upsert(newItem);
            isInsert.Should().BeFalse();

            // waiting for notification
            Thread.Sleep(TimeSpan.FromSeconds(1));

            receivedItems.Should().NotBeNull();
            receivedItems.Should().HaveCount(1);
            receivedItems[0].Id.Should().Be(newItem.Id);
            receivedItems[0].Name.Should().Be(newItem.Name);
            receivedItems[0].Price.Should().Be(newItem.Price);
            rawOnNextCount.Should().Be(3);
        }

        [Fact]
        public void Notify_Docuemnt_And_Collection_Subscription_When_The_Document_Modified()
        {
            using var db = new RealtimeLiteDatabase(new MemoryStream());
            Item receivedItem = null;
            List<Item> receivedItems = null;
            var newItem = new Item
            {
                Name = "Keyboard",
                Price = 100m
            };
            var newId = db.GetCollection<Item>("items").Insert(newItem);
            newItem.Id = newId.AsGuid;

            // docuement subscription
            using var docSub = db.Realtime.Collection<Item>("items").Id(newId).Subscribe(item => receivedItem = item);

            // collection subscription
            using var colSub = db.Realtime.Collection<Item>("items").Subscribe(items => receivedItems = items);

            // waiting for notification
            Thread.Sleep(TimeSpan.FromSeconds(1));

            // document subscription received
            receivedItem.Should().NotBeNull();
            receivedItem.Id.Should().Be(newItem.Id);
            receivedItem.Name.Should().Be(newItem.Name);
            receivedItem.Price.Should().Be(newItem.Price);

            // collection subscription received
            receivedItems.Should().NotBeNull();
            receivedItems.Should().HaveCount(1);
            receivedItems[0].Id.Should().Be(newItem.Id);
            receivedItems[0].Name.Should().Be(newItem.Name);
            receivedItems[0].Price.Should().Be(newItem.Price);

            // updating newItem
            newItem.Price = 99m;
            db.GetCollection<Item>("items").Update(newItem);

            // waiting for notification
            Thread.Sleep(TimeSpan.FromSeconds(1));

            // document subscription received
            receivedItem.Should().NotBeNull();
            receivedItem.Id.Should().Be(newItem.Id);
            receivedItem.Name.Should().Be(newItem.Name);
            receivedItem.Price.Should().Be(newItem.Price);

            // collection subscription received
            receivedItems.Should().NotBeNull();
            receivedItems.Should().HaveCount(1);
            receivedItems[0].Id.Should().Be(newItem.Id);
            receivedItems[0].Name.Should().Be(newItem.Name);
            receivedItems[0].Price.Should().Be(newItem.Price);
        }

        [Fact]
        public void Notify_Document_And_Collection_Subscription_When_Broadcasting()
        {
            using var db = new RealtimeLiteDatabase(new MemoryStream());
            Item receivedItem = null;
            List<Item> receivedItems = null;
            var newItem = new Item
            {
                Name = "Keyboard",
                Price = 100m
            };
            var newId = db.GetCollection<Item>("items").Insert(newItem);
            newItem.Id = newId.AsGuid;

            // docuement subscription
            using var docSub = db.Realtime.Collection<Item>("items").Id(newId).Subscribe(item => receivedItem = item);
            // collection subscription
            using var colSub = db.Realtime.Collection<Item>("items").Subscribe(items => receivedItems = items);

            // waiting for notification
            Thread.Sleep(TimeSpan.FromSeconds(1));

            // document subscription received
            receivedItem.Should().NotBeNull();
            receivedItem.Id.Should().Be(newItem.Id);
            receivedItem.Name.Should().Be(newItem.Name);
            receivedItem.Price.Should().Be(newItem.Price);

            // collection subscription received
            receivedItems.Should().NotBeNull();
            receivedItems.Should().HaveCount(1);
            receivedItems[0].Id.Should().Be(newItem.Id);
            receivedItems[0].Name.Should().Be(newItem.Name);
            receivedItems[0].Price.Should().Be(newItem.Price);

            // update with expression (will broadcast)
            int updatedNum = db.GetCollection<Item>("items").UpdateMany(item => new Item { Id = item.Id, Name = item.Name, Price = item.Price * 2 }, i => i.Price > 0);
            updatedNum.Should().Be(1);

            // waiting for notification
            Thread.Sleep(TimeSpan.FromSeconds(1));

            // document subscription received
            receivedItem.Should().NotBeNull();
            receivedItem.Id.Should().Be(newId.AsGuid);
            receivedItem.Name.Should().Be(newItem.Name);
            receivedItem.Price.Should().Be(newItem.Price * 2);

            // collection subscription received
            receivedItems.Should().NotBeNull();
            receivedItems.Should().HaveCount(1);
            receivedItems[0].Id.Should().Be(newId.AsGuid);
            receivedItems[0].Name.Should().Be(newItem.Name);
            receivedItems[0].Price.Should().Be(newItem.Price * 2);
        }

        [Fact]
        public void Notify_Null_Document_If_Id_Does_Not_Exist_When_Subscribing()
        {
            using var db = new RealtimeLiteDatabase(new MemoryStream());
            bool isNull = false;
            using var docSub = db.Realtime.Collection<Item>("items").Id(new BsonValue(Guid.NewGuid())).Subscribe(item => isNull = item is null);

            // waiting for notification
            Thread.Sleep(TimeSpan.FromSeconds(1));

            isNull.Should().BeTrue();
        }

        [Fact]
        public void Notify_Document_When_A_New_Data_Added()
        {
            var bsonvalue = new BsonValue();
            var id = Guid.NewGuid();
            using var db = new RealtimeLiteDatabase(new MemoryStream());
            Item receivedItem = new Item();
            using var docSub = db.Realtime.Collection<Item>("items").Id(new BsonValue(id)).Subscribe(item => receivedItem = item);

            // waiting for notification
            Thread.Sleep(TimeSpan.FromSeconds(1));

            receivedItem.Should().BeNull();

            var item = new Item
            {
                Id = id,
                Name = "Mouse",
                Price = 10m
            };

            db.GetCollection<Item>("items").Insert(item);

            // waiting for notification
            Thread.Sleep(TimeSpan.FromSeconds(1));

            receivedItem.Id.Should().Be(item.Id);
            receivedItem.Name.Should().Be(item.Name);
            receivedItem.Price.Should().Be(item.Price);
        }
    }
}