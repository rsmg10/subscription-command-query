using MediatR;
using SubscriptionQuery.QueryHandlers.PendingInvitationsReceived;
using SubscriptionQuery.Extensions;
using SubscriptionQuery.QueryHandlers.PendingInvitationsSent;

namespace SubscriptionQuery.QueryHandlers.PendingInvitationsSent
{
 
    public record PendingInvitationsSentQuery(
        Guid AccountId,
        Guid SubscriptionId,
        Guid UserId): IRequest<List<InvitationVm>>;

     
  
}
