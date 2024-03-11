namespace SubscriptionQuery.QueryHandlers.PendingInvitationsReceived
{
    public record ReceivedInvitation(
        string SubscriptionName,
        Guid Id,
        Guid SentBy,
        Guid OwnerId,
        DateTime CreatedAt);
 
}
