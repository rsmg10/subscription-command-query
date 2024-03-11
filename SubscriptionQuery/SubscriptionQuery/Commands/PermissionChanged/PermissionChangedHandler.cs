using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionQuery.Commands.InvitationRejected;
using SubscriptionQuery.Infrastructure.Presistance;

namespace SubscriptionQuery.Commands.PermissionChanged
{
    public class PermissionChangedHandler : IRequestHandler<PermissionChanged, bool>
    {
        private readonly ApplicationDatabase _db;
        private readonly ILogger<PermissionChangedHandler> _logger;

        public PermissionChangedHandler(ApplicationDatabase db, ILogger<PermissionChangedHandler> logger)
        {
            _db = db;
            _logger = logger;
        }
        public async Task<bool> Handle(PermissionChanged request, CancellationToken cancellationToken)
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
 

            userSubscription.Apply(request);

            return await _db.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
