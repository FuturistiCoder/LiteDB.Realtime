using LiteDB.Engine;
using LiteDB.Realtime.Notifications;
using System;
using System.Collections.Generic;

namespace LiteDB.Realtime
{
    public class RealtimeLiteEngine : ILiteEngine
    {
        private readonly ILiteEngine _engine;
        internal readonly NotificationService _notificationService = new NotificationService();

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
            var success = _engine.Commit();
            if (success)
            {
                _notificationService.Notify();
            }

            return success;

        }

        public int Delete(string collection, IEnumerable<BsonValue> ids)
        {
            try
            {
                _notificationService.Cache.AddDocuments(collection, ids);
                var result = _engine.Delete(collection, ids);
                _notificationService.Notify();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _notificationService.Clear();
            }
        }

        public int DeleteMany(string collection, BsonExpression predicate)
        {
            try
            {
                _notificationService.Cache.AddBroadcast(collection);
                var result = _engine.DeleteMany(collection, predicate);
                _notificationService.Notify();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _notificationService.Clear();
            }
        }

        public void Dispose()
        {
            _notificationService.Clear();
            _engine.Dispose();
        }

        public bool DropCollection(string name)
        {
            try
            {
                _notificationService.Cache.AddBroadcast(name);
                var result = _engine.DropCollection(name);
                _notificationService.Notify();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _notificationService.Clear();
            }
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
            try
            {
                _notificationService.Cache.AddDocuments(collection, docs);
                var result = _engine.Insert(collection, docs, autoId);
                _notificationService.Notify();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _notificationService.Clear();
            }
        }

        public IBsonDataReader Query(string collection, Query query)
        {
            return _engine.Query(collection, query);
        }

        public bool RenameCollection(string name, string newName)
        {

            try
            {
                _notificationService.Cache.AddBroadcast(name);
                _notificationService.Cache.AddBroadcast(newName);
                var result = _engine.RenameCollection(name, newName);
                _notificationService.Notify();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _notificationService.Clear();
            }
        }

        public bool Rollback()
        {
            _notificationService.Clear();
            return _engine.Rollback();
        }

        public int Update(string collection, IEnumerable<BsonDocument> docs)
        {
            try
            {
                _notificationService.Cache.AddDocuments(collection, docs);
                var result = _engine.Update(collection, docs);
                _notificationService.Notify();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _notificationService.Clear();
            }
        }

        public int UpdateMany(string collection, BsonExpression extend, BsonExpression predicate)
        {
            try
            {
                _notificationService.Cache.AddBroadcast(collection);
                var result = _engine.UpdateMany(collection, extend, predicate);
                _notificationService.Notify();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _notificationService.Clear();
            }
        }

        public int Upsert(string collection, IEnumerable<BsonDocument> docs, BsonAutoId autoId)
        {
            try
            {
                _notificationService.Cache.AddDocuments(collection, docs);
                var result = _engine.Upsert(collection, docs, autoId);
                _notificationService.Notify();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _notificationService.Clear();
            }
        }
    }
}
