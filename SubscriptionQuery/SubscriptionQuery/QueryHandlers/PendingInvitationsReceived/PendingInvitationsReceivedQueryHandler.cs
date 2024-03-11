﻿using Calzolari.Grpc.AspNetCore.Validation;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionQuery.Infrastructure.Presistance;

namespace SubscriptionQuery.QueryHandlers.PendingInvitationsReceived
{
    public class PendingInvitationsReceivedQueryHandler : IRequestHandler<PendingInvitationsReceivedQuery, List<InvitationVm>>
    {
        private readonly ApplicationDatabase _db;

        public PendingInvitationsReceivedQueryHandler(ApplicationDatabase db)
        {
            _db = db;
        }

        public async Task<List<InvitationVm>> Handle(PendingInvitationsReceivedQuery request, CancellationToken cancellationToken)
        {

            var userSubscriptions = await _db.Subscriptions
              .Include(sub => sub.Invitations)
              .Where(sub => sub.MemberId == request.UserId
                  && !sub.IsJoined)
              .ToListAsync(cancellationToken: cancellationToken);

            var invitations = userSubscriptions.Where(sub => sub.Invitations.MaxBy(x => x.DateCreated) == null ? false : (sub.Invitations.MaxBy(x => x.DateCreated)!.Status == Domain.Enums.InvitationStatus.Pending))
               .Select(sub => sub.Invitations.MaxBy(x => x.DateCreated));


            return invitations.Select(inv => new InvitationVm
            {
                CreatedAt = inv.DateCreated.ToTimestamp(),
                Id = inv.Id.ToString(),
                OwnerId = inv.UserSubscription.OwnerId.ToString(),
                SentBy = inv.UserSubscription.OwnerId.ToString(),
                SubscriptionName = "subscriptionName"
            }).ToList(); 
        }
    }
}
