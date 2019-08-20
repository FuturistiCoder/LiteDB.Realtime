namespace LiteDB.Realtime.Subscriptions
{
    public interface ICollectionSubscriptionBuilder
    {
        IDocumentSubscriptionBuilder<T> Collection<T>(string collection) where T : class;
    }
}