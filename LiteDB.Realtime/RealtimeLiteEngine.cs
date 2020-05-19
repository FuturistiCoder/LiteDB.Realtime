using LiteDB.Engine;
using LiteDB.Realtime.Notifications;
using System;
using System.Collections.Generic;

namespace LiteDB.Realtime
{
    public class RealtimeLiteEngine : ILiteEngine
    {
        private readonly ILiteEngine _engine;
        internal NotificationService NotificationService { get; } = new NotificationService();

        internal RealtimeLiteEngine(ILiteEngine engine)
        {
            _engine = engine;
        }

        public bool BeginTrans()
        {
            return _engine.BeginTrans();
        }

        public bool Commit()
        {
            var success = _engine.Commit();
            if (success)
            {
                NotificationService.Notify();
            }

            return success;

        }

        public int Delete(string collection, IEnumerable<BsonValue> ids)
        {
            try
            {
                NotificationService.Cache.AddDocuments(collection, ids);
                var result = _engine.Delete(collection, ids);
                NotificationService.Notify();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                NotificationService.Clear();
            }
        }

        public int DeleteMany(string collection, BsonExpression predicate)
        {
            try
            {
                NotificationService.Cache.AddBroadcast(collection);
                var result = _engine.DeleteMany(collection, predicate);
                NotificationService.Notify();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                NotificationService.Clear();
            }
        }

        public void Dispose()
        {
            NotificationService.Clear();
            _engine.Dispose();
        }

        public bool DropCollection(string name)
        {
            try
            {
                NotificationService.Cache.AddBroadcast(name);
                var result = _engine.DropCollection(name);
                NotificationService.Notify();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                NotificationService.Clear();
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
                NotificationService.Cache.AddDocuments(collection, docs);
                var result = _engine.Insert(collection, docs, autoId);
                NotificationService.Notify();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                NotificationService.Clear();
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
                NotificationService.Cache.AddBroadcast(name);
                NotificationService.Cache.AddBroadcast(newName);
                var result = _engine.RenameCollection(name, newName);
                NotificationService.Notify();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                NotificationService.Clear();
            }
        }

        public bool Rollback()
        {
            NotificationService.Clear();
            return _engine.Rollback();
        }

        public int Update(string collection, IEnumerable<BsonDocument> docs)
        {
            try
            {
                NotificationService.Cache.AddDocuments(collection, docs);
                var result = _engine.Update(collection, docs);
                NotificationService.Notify();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                NotificationService.Clear();
            }
        }

        public int UpdateMany(string collection, BsonExpression extend, BsonExpression predicate)
        {
            try
            {
                NotificationService.Cache.AddBroadcast(collection);
                var result = _engine.UpdateMany(collection, extend, predicate);
                NotificationService.Notify();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                NotificationService.Clear();
            }
        }

        public int Upsert(string collection, IEnumerable<BsonDocument> docs, BsonAutoId autoId)
        {
            try
            {
                NotificationService.Cache.AddDocuments(collection, docs);
                var result = _engine.Upsert(collection, docs, autoId);
                NotificationService.Notify();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                NotificationService.Clear();
            }
        }

        public int Checkpoint()
        {
            return _engine.Checkpoint();
        }

        public long Rebuild(RebuildOptions options)
        {
            return _engine.Rebuild(options);
        }

        public BsonValue Pragma(string name)
        {
            return _engine.Pragma(name);
        }

        public bool Pragma(string name, BsonValue value)
        {
            return _engine.Pragma(name, value);
        }
    }
}
