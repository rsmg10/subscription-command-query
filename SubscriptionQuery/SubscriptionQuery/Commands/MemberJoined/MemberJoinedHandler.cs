using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionQuery.Commands.InvitationRejected;
using SubscriptionQuery.Infrastructure.Presistance;
using SubscriptionQuery.Infrastructure.Presistance.Entities;

namespace SubscriptionQuery.Commands.MemberJoined
{
    public class MemberJoinedHandler : IRequestHandler<MemberJoined, bool>
    {
        private readonly ApplicationDatabase _db;
        private readonly ILogger<MemberJoinedHandler> _logger;

        public MemberJoinedHandler(ApplicationDatabase db, ILogger<MemberJoinedHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<bool> Handle(MemberJoined request, CancellationToken cancellationToken)
        {

            var userSubscription = await _db.Subscriptions
                .Include(us => us.Invitations)
                .FirstOrDefaultAsync(s
                => s.Id == request.AggregateId, cancellationToken);

            if (((userSubscription == null || userSubscription.Sequence < request.Sequence - 1) && request.Data.JoinedBy is Domain.Enums.JoinedBy.Invitation) 
                || (userSubscription == null || userSubscription.Sequence < request.Sequence - 1) && request.Data.JoinedBy is Domain.Enums.JoinedBy.Admin && request.Sequence != 1)
            {
                _logger.LogWarning(
                    "Event not handled, AggregateId: {AggregateId}, Sequence: {Sequence}.",
                    request.AggregateId,
                    request.Sequence);
                return false;
            }
            
            var isNew = userSubscription is null;
            userSubscription ??= new UserSubscription();

            if (request.Sequence <= userSubscription.Sequence)
                return true;


            userSubscription.Apply(request);

            if (isNew)
            {
                await _db.Subscriptions.AddAsync(userSubscription, cancellationToken);
            }
            
            await _db.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
