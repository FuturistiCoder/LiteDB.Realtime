namespace LiteDB.Realtime.Subscriptions
{
    internal class CollectionSubscriptionBuilder : ICollectionSubscriptionBuilder
    {
        private readonly RealtimeLiteDatabase _database;

        public CollectionSubscriptionBuilder(RealtimeLiteDatabase database)
        {
            _database = database;
        }

        public IDocumentSubscriptionBuilder<T> Collection<T>(string collection) where T : class
        {
            var subscription = new Subscription<T>(_database)
            {
                Collection = collection
            };
            return new DocumentSubscriptionBuilder<T>(_database, subscription);
        }
    }
}
