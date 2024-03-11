using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionQuery.Infrastructure.Presistance;
using SubscriptionQuery.Extensions;

namespace SubscriptionQuery.QueryHandlers.JoinedSubscription
{
    public class JoinedSubscriptionQueryHandler : IRequestHandler<JoinedSubscriptionQuery, JoinedSubscriptionResponse>
    { 
        private readonly ApplicationDatabase _db;

        public JoinedSubscriptionQueryHandler(ApplicationDatabase db)
        {
            _db = db;
        }

        public async Task<JoinedSubscriptionResponse> Handle(JoinedSubscriptionQuery request, CancellationToken cancellationToken)
        {
            var subscriptions = await _db.Subscriptions
                .Where(s => s.MemberId == request.UserId && s.IsJoined)
                .Select(sub
                        => new JoinedSubscription
                        (
                            sub.Id,
                            "",
                            sub.MemberId,
                            sub.OwnerId,
                            sub.DateCreated,
                    sub.Permissions
                )).ToListAsync(cancellationToken);

            return new JoinedSubscriptionResponse
            {
                Subscriptions =
                {
                    subscriptions.Select(x=> new SubscriptionVm
                    {
                        Id = x.Id.ToString(),
                        CreatedAt = x.CreatedAt.ToUtcTimestamp(),
                        Name = x.Name,
                        OwnerId = x.OwnerId.ToString(),
                        Permissions = (long)x.Permissions,
                        UserId = x.UserId.ToString(),
                    })
                }
            };

        }
    }
}
