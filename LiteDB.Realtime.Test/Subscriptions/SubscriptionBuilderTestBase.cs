using LiteDB.Realtime.Notifications;
using System.IO;

namespace LiteDB.Realtime.Test.Subscriptions
{
    public class SubscriptionBuilderTestBase
    {
        protected RealtimeLiteDatabase _db;

        public SubscriptionBuilderTestBase()
        {
            _db = new RealtimeLiteDatabase(new MemoryStream());
        }
    }
}
