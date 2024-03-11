using SubscriptionQuery.QueryHandlers.JoinedSubscription;
using SubscriptionQuery.QueryHandlers.PendingInvitationsReceived;
using SubscriptionQuery.QueryHandlers.MembersInSubscription;
using SubscriptionQuery.QueryHandlers.PendingInvitationsSent;

namespace SubscriptionQuery.Extensions
{
    public static class QueryExtension
    {
        public static JoinedSubscriptionQuery ToQuery( this JoinedSubscriptionRequest request)
        {
            return new(request.UserId.ToGuid());
        }

        public static PendingInvitationsReceivedQuery ToQuery( this PendingInvitationsReceivedRequest request)
        {
            return new(request.UserId.ToGuid());
        }
        public static MembersInSubscriptionQuery ToQuery( this MembersInSubscriptionRequest request)
        {
            return new(
                request.AccountId.ToGuid(),
                request.SubscriptionId.ToGuid(),
                request.UserId.ToGuid());
        }

        public static PendingInvitationsSentQuery ToQuery( this PendingInvitationsSentRequest request)
        {
            return new(
                request.AccountId.ToGuid(),
                request.SubscriptionId.ToGuid(),
                request.UserId.ToGuid());
        }
         
    }   
}
