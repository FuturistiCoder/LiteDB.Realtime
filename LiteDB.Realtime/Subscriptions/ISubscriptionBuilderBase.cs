using System;
using System.Collections.Generic;
using System.Text;

namespace LiteDB.Realtime.Subscriptions
{
    public interface ISubscriptionBuilderBase
    {
         internal ISubscription Subscription { get; }
    }
}
