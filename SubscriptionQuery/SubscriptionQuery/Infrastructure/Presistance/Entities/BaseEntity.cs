namespace SubscriptionQuery.Infrastructure.Presistance.Entities;

public abstract class BaseEntity
{
    public int  Sequence { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
}