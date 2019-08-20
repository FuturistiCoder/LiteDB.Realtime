using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDB.Realtime
{
    internal class Notifications
    {
        /// <summary>
        /// Broadcast notifications (for collection and all documents)
        /// CollectionName, _
        /// </summary>
        public ConcurrentDictionary<string, byte> Broadcasts { get; } = new ConcurrentDictionary<string, byte>();
        /// <summary>
        /// Collection notifications
        /// CollectionName, _
        /// </summary>
        public ConcurrentDictionary<string, byte> Collections { get; } = new ConcurrentDictionary<string, byte>();

        /// <summary>
        /// Document notifications
        /// (CollectionName, _id), _
        /// </summary>
        public ConcurrentDictionary<(string, BsonValue), byte> Documents { get; } = new ConcurrentDictionary<(string, BsonValue), byte>();

        public void NotifyCollection(string collectionName)
        {
            if (!Broadcasts.ContainsKey(collectionName))
            {
                Collections.TryAdd(collectionName, default);
            }
        }

        public void BroadcastCollectionAndDocument(string collectionName)
        {
            Broadcasts.TryAdd(collectionName, default);
            var CollsToDelete = Collections.Keys.Where(key => key == collectionName);
            foreach(var key in CollsToDelete)
            {
                Collections.TryRemove(key, out byte _);
            }

            var DocsToDelete = Documents.Keys.Where(key => key.Item1 == collectionName);
            foreach(var key in DocsToDelete)
            {
                Documents.TryRemove(key, out byte _);
            }
        }

        public void NotifyDocument(string collectionName, IEnumerable<BsonDocument> documents)
        {
            var ids = documents.Select(doc => doc["_id"]);
            NotifyDocument(collectionName, ids);
        }

        public void NotifyDocument(string collectionName, IEnumerable<BsonValue> ids)
        {
            if (Broadcasts.TryGetValue(collectionName, out _))
            {
                return;
            }

            foreach (var id in ids)
            {
                Documents[(collectionName, id)] = default;
            }
        }

        public void Clear()
        {
            Collections.Clear();
            Documents.Clear();
        }

    }
}
