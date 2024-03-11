using MediatR;
using SubscriptionQuery.QueryHandlers.PendingInvitationsSent;

namespace SubscriptionQuery.QueryHandlers.PendingInvitationsReceived
{

    public record PendingInvitationsReceivedQuery(Guid UserId): IRequest<List<InvitationVm>>;


}
