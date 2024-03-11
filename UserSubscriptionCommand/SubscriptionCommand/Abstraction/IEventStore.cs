using SubscriptionCommand.Domain;
using SubscriptionCommand.Events;

namespace SubscriptionCommand.Abstraction
{
    public interface IEventStore
    {
       public  Task<List<Event>> GetAllAsync(Guid aggregateId, CancellationToken cancellationToken);
       public Task CommitAsync(UserSubscription userSubscription, CancellationToken cancellationToken);
    }
}
