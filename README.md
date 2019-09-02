# LiteDB.Realtime

[![Build Status](https://dev.azure.com/FuturistiCoder/LiteDB.Realtime/_apis/build/status/FuturistiCoder.LiteDB.Realtime?branchName=master)](https://dev.azure.com/FuturistiCoder/LiteDB.Realtime/_build/latest?definitionId=9&branchName=master)
[![nuget](https://img.shields.io/nuget/v/LiteDB.Realtime.svg)](https://www.nuget.org/packages/LiteDB.Realtime/)

LiteDB.Realtime is a [LiteDB](https://github.com/mbdavid/LiteDB) with realtime notifications.

## Get started

You can subscribe to a document or a total collection with [System.Reactive](https://www.nuget.org/packages/System.Reactive) easily.

```C#
List<Item> receivedItems = null;
Item receivedItem = null;

using (var db = new RealtimeLiteDatabase(new MemoryStream()))
{
    var newItem = new Item
    {
        Id = Guid.NewGuid(),
        Name = "Keyboard",
        Price = 100m
    };

    // docuement subscription
    // subscribe with System.Reactive extensions
    db.Realtime.Collection<Item>("items").Id(new BsonValue(newItem.Id)).Subscribe(item => receivedItem = item); 

    // collection subscription
    // subscribe with System.Reactive extensions 
    db.Realtime.Collection<Item>("items").Subscribe(items => receivedItems = items);

    // insert new item
    db.GetCollection<Item>("items").Insert(newItem);

    // receivedItems: [ newItem ]
    // receivedItem: newItem
}

```

