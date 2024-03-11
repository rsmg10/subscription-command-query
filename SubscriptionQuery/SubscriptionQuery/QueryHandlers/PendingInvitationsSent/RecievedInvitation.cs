using SubscriptionQuery.QueryHandlers.PendingInvitationsReceived;
namespace SubscriptionQuery.QueryHandlers.PendingInvitationsSent
{
    public record SentInvitation(
        string SubscriptionName,
        Guid Id,
        Guid SentBy,
        Guid OwnerId,
        DateTime CreatedAt);
 
}
