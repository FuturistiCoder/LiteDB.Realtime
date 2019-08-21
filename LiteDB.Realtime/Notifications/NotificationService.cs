using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDB.Realtime.Notifications
{
    internal class NotificationService
    {
        public Action<NotificationCache> NotificationCallback { get; internal set; }

        public NotificationCache Cache { get; private set; } = new NotificationCache();


        /// <summary>
        /// Notify and clean cache
        /// </summary>
        public void Notify()
        {
            var copy = Cache;
            Cache = new NotificationCache();
            NotificationCallback?.Invoke(copy);
        }

        /// <summary>
        /// Clear notification cache
        /// </summary>
        public void Clear()
        {
            Cache.Clear();
        }

    }
}
