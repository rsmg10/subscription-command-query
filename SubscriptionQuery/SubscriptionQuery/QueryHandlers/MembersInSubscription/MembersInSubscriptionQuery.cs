using MediatR;
using SubscriptionQuery.QueryHandlers.MembersInSubscription;
using SubscriptionQuery.Extensions;

namespace SubscriptionQuery.QueryHandlers.MembersInSubscription
{ 
        public record MembersInSubscriptionQuery(Guid AccountId,
                                                 Guid SubscriptionId,
                                                 Guid UserId): IRequest<MembersInSubscriptionResponse>;
     
  
}
