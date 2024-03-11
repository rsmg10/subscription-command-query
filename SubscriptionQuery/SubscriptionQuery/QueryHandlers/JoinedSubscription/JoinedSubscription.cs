using SubscriptionQuery.Domain.Enums;

namespace SubscriptionQuery.QueryHandlers.JoinedSubscription
{

    public record JoinedSubscription(
        Guid Id,
        string Name,
        Guid UserId,
        Guid OwnerId,
        DateTime CreatedAt,
        Permissions Permissions);







}
