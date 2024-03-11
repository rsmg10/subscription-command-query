using MediatR;

namespace SubscriptionQuery.QueryHandlers.JoinedSubscription
{
    public record JoinedSubscriptionQuery(Guid UserId) : IRequest<JoinedSubscriptionResponse>;


}
