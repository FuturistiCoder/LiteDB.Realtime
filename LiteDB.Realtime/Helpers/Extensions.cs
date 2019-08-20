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
                LimitSize = connectionString.LimitSize,
                UtcDate = connectionString.UtcDate,
                Timeout = connectionString.Timeout,
                ReadOnly = connectionString.ReadOnly
            };

            // create engine implementation as Connection Type
            if (connectionString.Mode == ConnectionMode.Embedded)
            {
                return new RealtimeLiteEngine(new LiteEngine(settings));
            }
            else
            {
                throw new NotImplementedException();
            }
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
