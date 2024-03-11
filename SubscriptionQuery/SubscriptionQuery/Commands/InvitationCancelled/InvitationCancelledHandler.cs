using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionQuery.Commands.InvitationAccepted;
using SubscriptionQuery.Infrastructure.Presistance;

namespace SubscriptionQuery.Commands.InvitationCancelled
{
    public class InvitationCancelledHandler : IRequestHandler<InvitationCancelled, bool>
    {
        private readonly ApplicationDatabase _db;
        private readonly ILogger<InvitationCancelledHandler> _logger;

        public InvitationCancelledHandler(ApplicationDatabase db, ILogger<InvitationCancelledHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<bool> Handle(InvitationCancelled request, CancellationToken cancellationToken)
        {

            var userSubscription = await _db.Subscriptions
                .Include(us => us.Invitations)
                .FirstOrDefaultAsync(s
                => s.Id == request.AggregateId, cancellationToken);


            if (userSubscription == null || userSubscription.Sequence < request.Sequence - 1)
            {
                _logger.LogWarning(
                    "Event not handled, AggregateId: {AggregateId}, Sequence: {Sequence}.",
                    request.AggregateId,
                    request.Sequence
);
                return false;
            }

            if (request.Sequence <= userSubscription.Sequence)
                return true;

            if (request.Sequence != userSubscription.Sequence + 1)
                return false;

            userSubscription.Apply(request);

            return await _db.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
