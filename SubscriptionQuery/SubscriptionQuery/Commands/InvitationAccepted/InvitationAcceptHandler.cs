using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionQuery.Events;
using SubscriptionQuery.Infrastructure.Presistance;

namespace SubscriptionQuery.Commands.InvitationAccepted
{
    public class InvitationAcceptHandler : IRequestHandler<InvitationAccepted, bool>
    {
        private readonly ApplicationDatabase _db;
        private readonly ILogger<InvitationAcceptHandler> _logger;
        public InvitationAcceptHandler(ApplicationDatabase db, ILogger<InvitationAcceptHandler> logger)
        {
            _db = db;
            _logger = logger;
        }
        public async Task<bool> Handle(InvitationAccepted request, CancellationToken cancellationToken)
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
                    request.Sequence);
                return false;
            }
            if (request.Sequence <= userSubscription.Sequence)
                return true;



            userSubscription.Apply(request);

            return await _db.SaveChangesAsync(cancellationToken) > 0;

        }
    }
}
