using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
