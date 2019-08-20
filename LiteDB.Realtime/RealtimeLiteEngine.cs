using LiteDB.Engine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LiteDB.Realtime
{
    public class RealtimeLiteEngine : ILiteEngine
    {
        private readonly ILiteEngine _engine;
        internal Action<Notifications> NotificationCallback { get; set; }
        private Notifications _notifications = new Notifications();

        internal RealtimeLiteEngine(ILiteEngine engine)
        {
            _engine = engine;
        }

        public int Analyze(string[] collections)
        {
            return _engine.Analyze(collections);
        }

        public bool BeginTrans()
        {
            return _engine.BeginTrans();
        }

        public void Checkpoint()
        {
            _engine.Checkpoint();
        }

        public bool Commit()
        {

            var copy = _notifications;
            _notifications = new Notifications();
            var success = _engine.Commit();
            if (success)
            {
                NotificationCallback?.Invoke(copy);
            }

            return success;
        }

        public int Delete(string collection, IEnumerable<BsonValue> ids)
        {
            _notifications.NotifyDocument(collection, ids);
            return _engine.Delete(collection, ids);
        }

        public int DeleteMany(string collection, BsonExpression predicate)
        {
            _notifications.BroadcastCollectionAndDocument(collection);
            return _engine.DeleteMany(collection, predicate);
        }

        public void Dispose()
        {
            _notifications.Clear();
            _engine.Dispose();
        }

        public bool DropCollection(string name)
        {
            _notifications.BroadcastCollectionAndDocument(name);
            return _engine.DropCollection(name);
        }

        public bool DropIndex(string collection, string name)
        {
            return _engine.DropIndex(collection, name);
        }

        public bool EnsureIndex(string collection, string name, BsonExpression expression, bool unique)
        {
            return _engine.EnsureIndex(collection, name, expression, unique);
        }

        public int Insert(string collection, IEnumerable<BsonDocument> docs, BsonAutoId autoId)
        {
            _notifications.NotifyCollection(collection);
            return _engine.Insert(collection, docs, autoId);
        }

        public IBsonDataReader Query(string collection, Query query)
        {
            return _engine.Query(collection, query);
        }

        public bool RenameCollection(string name, string newName)
        {
            _notifications.BroadcastCollectionAndDocument(name);
            _notifications.BroadcastCollectionAndDocument(newName);
            return _engine.RenameCollection(name, newName);
        }

        public bool Rollback()
        {
            _notifications.Clear();
            return _engine.Rollback();
        }

        public int Update(string collection, IEnumerable<BsonDocument> docs)
        {
            _notifications.NotifyDocument(collection, docs);
            return _engine.Update(collection, docs);
        }

        public int UpdateMany(string collection, BsonExpression extend, BsonExpression predicate)
        {
            _notifications.BroadcastCollectionAndDocument(collection);
            return _engine.UpdateMany(collection, extend, predicate);
        }

        public int Upsert(string collection, IEnumerable<BsonDocument> docs, BsonAutoId autoId)
        {
            _notifications.NotifyDocument(collection, docs);
            return _engine.Upsert(collection, docs, autoId);
        }
    }
}
