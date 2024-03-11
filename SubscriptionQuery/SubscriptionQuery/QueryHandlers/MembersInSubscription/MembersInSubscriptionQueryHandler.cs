using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionQuery.Infrastructure.Presistance;
using SubscriptionQuery.QueryHandlers.MembersInSubscription;
using SubscriptionQuery.Extensions;

namespace SubscriptionQuery.QueryHandlers.MembersInSubscription
{
    public class MembersInSubscriptionQueryHandler : IRequestHandler<MembersInSubscriptionQuery, MembersInSubscriptionResponse>
    {
        private readonly ApplicationDatabase _db;

        public MembersInSubscriptionQueryHandler(ApplicationDatabase db)
        {
            _db = db;
        }

        public async Task<MembersInSubscriptionResponse> Handle(MembersInSubscriptionQuery request, CancellationToken cancellationToken)
        {
            var userSubscriptions = await _db.Subscriptions.Where(sub => request.AccountId == sub.AccountId
            && request.SubscriptionId == sub.SubscriptionId
            && request.UserId == sub.OwnerId)
                .Select(sub => new MemberInSubscription(sub.MemberId, ""))
                .ToListAsync(cancellationToken);

            return new MembersInSubscriptionResponse
            {
                Members = {userSubscriptions.Select(x => new Member
                {
                    Id = x.UserId.ToString(),
                    Name = x.Name,
                }) }
            };

        }
    }
}
