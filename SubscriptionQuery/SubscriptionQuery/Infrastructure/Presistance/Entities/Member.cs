namespace SubscriptionQuery.Infrastructure.Presistance.Entities;

public class Member : BaseEntity
{
    public Guid Id { get; set; } 
    public ICollection<UserSubscription> Subscriptions { get; set; }
}