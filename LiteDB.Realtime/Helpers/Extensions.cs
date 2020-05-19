using LiteDB.Engine;
using System;
using System.IO;

namespace LiteDB.Realtime.Helpers
{
    public static class Extensions
    {
        public static ILiteEngine CreateRealtimeEngine(this ConnectionString connectionString)
        {
            var settings = new EngineSettings
            {
                Filename = connectionString.Filename,
                Password = connectionString.Password,
                InitialSize = connectionString.InitialSize,
                ReadOnly = connectionString.ReadOnly,
                Collation = connectionString.Collation
            };

            // create engine implementation as Connection Type
            return connectionString.Connection switch
            {
                ConnectionType.Direct => new RealtimeLiteEngine(new LiteEngine(settings)),
                ConnectionType.Shared => new RealtimeLiteEngine(new SharedEngine(settings)),
                _ => throw new NotImplementedException()
            };
        }

        public static ILiteEngine CreateRealtimeEngine(this Stream stream)
        {
            var settings = new EngineSettings
            {
                DataStream = stream
            };

            return new RealtimeLiteEngine(new LiteEngine(settings));
        }
    }
}