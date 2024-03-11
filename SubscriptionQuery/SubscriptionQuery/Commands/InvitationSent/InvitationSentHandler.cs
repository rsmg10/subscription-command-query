using MediatR;
using Microsoft.EntityFrameworkCore;
using Polly;
using SubscriptionQuery.Commands.InvitationRejected;
using SubscriptionQuery.Domain.Enums;
using SubscriptionQuery.Extensions;
using SubscriptionQuery.Infrastructure.Presistance;
using SubscriptionQuery.Infrastructure.Presistance.Entities;

namespace SubscriptionQuery.Commands.InvitationSent
{
    public class InvitationSentHandler : IRequestHandler<InvitationSent, bool>
    {
        private readonly ApplicationDatabase _db;
        private readonly ILogger<InvitationSentHandler> _logger;

        public InvitationSentHandler(ApplicationDatabase context, ILogger<InvitationSentHandler> logger)
        {
            _db = context;
            _logger = logger;
        }

        public async Task<bool> Handle(InvitationSent request, CancellationToken cancellationToken)
        { 
 
            if (await _db.Invitations.AsNoTracking()
                .AnyAsync(i => i.Id == request.Data.InvitationId && i.UserSubscriptionId == request.AggregateId && i.Status == InvitationStatus.Pending, cancellationToken: cancellationToken))
                return true;

            var userSubscription = await _db.Subscriptions.Include(x=> x.Invitations)
                .FirstOrDefaultAsync(s 
                => s.Id == request.AggregateId, cancellationToken);

            var isNew = userSubscription == null;

            userSubscription ??= UserSubscription.Create(request);

            if (request.Sequence <= userSubscription.Sequence)
                return true;


            if (userSubscription.Sequence < request.Sequence - 1)
            {
                _logger.LogWarning(
                    "Event not handled, AggregateId: {AggregateId}, Sequence: {Sequence}.",
                    request.AggregateId,
                    request.Sequence);
                return false;
            }

            userSubscription.Apply(request);


            if (isNew)
                await _db.Subscriptions.AddAsync(userSubscription, cancellationToken);

            return await _db.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
