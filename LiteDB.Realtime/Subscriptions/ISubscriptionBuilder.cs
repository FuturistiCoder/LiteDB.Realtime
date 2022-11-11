namespace LiteDB.Realtime.Subscriptions
{
    public interface ISubscriptionBuilder
    {
        ICollectionSubscriptionBuilder<T> Collection<T>(string collection);

        ICollectionSubscriptionBuilder<T> Collection<T>();
    }
}