using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionQuery.Commands.InvitationAccepted;
using SubscriptionQuery.Infrastructure.Presistance;

namespace SubscriptionQuery.Commands.InvitationRejected
{
    public record InvitationRejectedHandler : IRequestHandler<InvitationRejected, bool>
    {
        private readonly ApplicationDatabase _db;
        private readonly ILogger<InvitationRejectedHandler> _logger;

        public InvitationRejectedHandler(ApplicationDatabase db, ILogger<InvitationRejectedHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<bool> Handle(InvitationRejected request, CancellationToken cancellationToken)
        {
            var userSubscription = await _db.Subscriptions
                    .Include(us => us.Invitations)
                    .FirstOrDefaultAsync(s
                    => s.Id == request.AggregateId, cancellationToken);

            if (userSubscription == null)
                return false;

            if (request.Sequence <= userSubscription.Sequence)
                return true;

            if (request.Sequence != userSubscription.Sequence + 1)
                return false;

            userSubscription.Apply(request);

            return await _db.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
