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
        private readonly ConcurrentDictionary<string, byte> _broadcasts  = new ConcurrentDictionary<string, byte>();
        public ICollection<string> Broadcasts => _broadcasts.Keys;
        /// <summary>
        /// Collection notifications
        /// CollectionName, _
        /// </summary>
        private readonly ConcurrentDictionary<string, byte> _collections = new ConcurrentDictionary<string, byte>();
        public ICollection<string> Collections => _collections.Keys;

        /// <summary>
        /// Document notifications
        /// (CollectionName, _id), _
        /// </summary>
        private readonly ConcurrentDictionary<(string, BsonValue), byte> _documents = new ConcurrentDictionary<(string, BsonValue), byte>();
        public ICollection<(string, BsonValue)> Documents => _documents.Keys;

        public void NotifyCollection(string collectionName)
        {
            if (!_broadcasts.ContainsKey(collectionName))
            {
                _collections.TryAdd(collectionName, default);
            }
        }

        public void BroadcastCollectionAndDocument(string collectionName)
        {
            _broadcasts.TryAdd(collectionName, default);
            var CollsToDelete = _collections.Keys.Where(key => key == collectionName);
            foreach(var key in CollsToDelete)
            {
                _collections.TryRemove(key, out byte _);
            }

            var DocsToDelete = _documents.Keys.Where(key => key.Item1 == collectionName);
            foreach(var key in DocsToDelete)
            {
                _documents.TryRemove(key, out byte _);
            }
        }

        public void NotifyDocument(string collectionName, IEnumerable<BsonDocument> documents)
        {
            var ids = documents.Select(doc => doc["_id"]);
            NotifyDocument(collectionName, ids);
        }

        public void NotifyDocument(string collectionName, IEnumerable<BsonValue> ids)
        {
            if (_broadcasts.TryGetValue(collectionName, out _))
            {
                return;
            }

            foreach (var id in ids)
            {
                _documents[(collectionName, id)] = default;
            }
        }

        public void Clear()
        {
            _broadcasts.Clear();
            _collections.Clear();
            _documents.Clear();
        }

    }
}
