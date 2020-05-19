using LiteDB.Engine;
using LiteDB.Realtime.Helpers;
using LiteDB.Realtime.Notifications;
using LiteDB.Realtime.Subscriptions;
using System;
using System.IO;

namespace LiteDB.Realtime
{
    public class RealtimeLiteDatabase : LiteDatabase
    {
        private readonly RealtimeLiteEngine _engine;
        internal NotificationService NotificationService => _engine.NotificationService;

        public RealtimeLiteDatabase(string connectionString, BsonMapper? mapper = null)
            : this(new ConnectionString(connectionString), mapper)
        {
        }

        public RealtimeLiteDatabase(ConnectionString connectionString, BsonMapper? mapper = null)
            : this(connectionString.CreateRealtimeEngine(), mapper)
        {
        }

        public RealtimeLiteDatabase(Stream stream, BsonMapper? mapper = null)
            : this(stream.CreateRealtimeEngine(), mapper)
        {
        }

        public RealtimeLiteDatabase(ILiteEngine engine, BsonMapper? mapper = null)
            : base(engine, mapper)
        {
            if (engine is RealtimeLiteEngine realtimeEngine)
            {
                _engine = realtimeEngine;
                _engine.NotificationService.Init(this);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public ISubscriptionBuilder Realtime => _engine.NotificationService.SubscriptionBuilder();
    }
}