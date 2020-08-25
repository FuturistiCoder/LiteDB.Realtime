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
    db.Realtime.Collection<Item>("items").Id(newItem.Id).Subscribe(item => receivedItem = item);

    // collection subscription
    // subscribe with System.Reactive extensions 
    db.Realtime.Collection<Item>("items").Subscribe(items => receivedItems = items);

    // insert new item
    db.GetCollection<Item>("items").Insert(newItem);

    // receivedItems: [ newItem ]
    // receivedItem: newItem
}

```

If you change the collection quickly, and you want to throttle the notifications.

```C#
// raw collection subscription
// this subscription will NOT retrieve the list of items each time, but a ILiteCollection<Item> instead.
// you can do what you want with this ILiteCollection<Item>
// subscribe with System.Reactive extensions
db.Reactive.Collection<Item>("items").Raw.Subscribe(col => /* col is type of ILiteCollection<Item> */);

// you can throttle the notifications by 1 second for example.
db.Realtime
    .Collection<Item>("items")
    .Raw
    .Throttle(TimeSpan.FromSecond(1)) // System.Reactive
    .Subscribe(col =>
    {
        var list = col.Query().OrderBy(i => i.Id).Limit(10).ToList();
        Update(list);
    });
```
