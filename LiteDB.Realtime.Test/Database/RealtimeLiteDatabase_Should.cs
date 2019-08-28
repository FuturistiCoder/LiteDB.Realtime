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
        class Item
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }

        }
        [Fact]
        public void Be_Created_By_Constructors()
        {
            var fileName = Path.GetTempPath() + Path.GetRandomFileName();
            new RealtimeLiteDatabase(fileName);

            new RealtimeLiteDatabase(new ConnectionString
            {
                Filename = fileName
            });

            new RealtimeLiteDatabase(new MemoryStream { });

            new RealtimeLiteDatabase(new RealtimeLiteEngine(new LiteEngine()));
        }

        [Fact]
        public void Notify_Collection_Subscription_When_A_New_Data_Added()
        {
            using (var db = new RealtimeLiteDatabase(new MemoryStream()))
            {
                List<Item> receivedItems = null;
                // collection subscription
                db.Realtime.Collection<Item>("items").Subscribe(items => receivedItems = items);

                //waiting for notification
                Thread.Sleep(TimeSpan.FromSeconds(1));
                receivedItems.Should().BeEmpty();

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
            }
        }

        [Fact]
        public void Notify_Docuemnt_And_Collection_Subscription_When_The_Document_Modified()
        {
            using (var db = new RealtimeLiteDatabase(new MemoryStream()))
            {
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
                db.Realtime.Collection<Item>("items").Id(newId).Subscribe(item => receivedItem = item);

                // collection subscription
                db.Realtime.Collection<Item>("items").Subscribe(items => receivedItems = items);

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
        }

        [Fact]
        public void Notify_Docuemnt_And_Collection_Subscription_When_Broadcasting()
        {
            using (var db = new RealtimeLiteDatabase(new MemoryStream()))
            {
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
                db.Realtime.Collection<Item>("items").Id(newId).Subscribe(item => receivedItem = item);
                // collection subscription
                db.Realtime.Collection<Item>("items").Subscribe(items => receivedItems = items);

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
        }
    }
}
