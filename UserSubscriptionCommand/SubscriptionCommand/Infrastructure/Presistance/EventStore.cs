using Microsoft.EntityFrameworkCore;
using SubscriptionCommand.Abstraction;
using SubscriptionCommand.Domain;
using SubscriptionCommand.Events;
using SubscriptionCommand.Infrastructure.MessageBus;
using System.Text.Json;

namespace SubscriptionCommand.Infrastructure.Presistance
{
    public class EventStore : IEventStore
    {
        private readonly ApplicationDatabase _db;
        private readonly AzureMessageBus _publisher;

        public EventStore(ApplicationDatabase db, AzureMessageBus publisher)
        {
            _db = db;
            _publisher = publisher;
        }

        public async Task CommitAsync(UserSubscription userSubscription, CancellationToken cancellationToken)
        {
            var events = userSubscription.GetUncommittedEvents();

            var dbEvents = events.Select(x => new EventEntity
            {
                AggregateId = x.AggregateId,
                Data = JsonSerializer.Serialize((object)((dynamic)x).Data),
                DateTime = x.DateTime,
                Id = x.Id,
                Sequence = x.Sequence,
                Type = x.Type,
                UserId = x.UserId,
                Version = x.Version,
            }).ToList();

            var messages = dbEvents.Select(x => new Outbox(x)).ToList();
            //await _db.Events.AddRangeAsync(dbEvents, cancellationToken);
            await _db.Outbox.AddRangeAsync(messages, cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);

            _publisher.Publish();

        }

        public async Task<List<Event>> GetAllAsync(Guid aggregateId, CancellationToken cancellationToken)
        {

            var eventEntities = await _db.Events.Where(e => e.AggregateId == aggregateId)
                .OrderBy(e => e.Sequence).ToListAsync(cancellationToken: cancellationToken);

            List<Event> events = eventEntities.Select(e => MapToEvent(e)).ToList();
            return events;
        }

  

        public Event MapToEvent(EventEntity e)
        { 
            return e.Type switch
            {
                nameof(InvitationSent) => new InvitationSent(e.AggregateId, JsonSerializer.Deserialize<InvitationSentData>(e.Data), e.DateTime, e.Sequence, e.UserId, e.Version),
                nameof(InvitationAccepted) => new InvitationAccepted(e.AggregateId, JsonSerializer.Deserialize<InvitationAcceptedData>(e.Data), e.DateTime, e.Sequence, e.UserId, e.Version),
                nameof(InvitationCancelled) => new InvitationCancelled(e.AggregateId, JsonSerializer.Deserialize<InvitationCancelledData>(e.Data), e.DateTime, e.Sequence, e.UserId, e.Version),
                nameof(InvitationRejected) => new InvitationRejected(e.AggregateId, JsonSerializer.Deserialize<InvitationRejectedData>(e.Data), e.DateTime, e.Sequence, e.UserId, e.Version),
                nameof(MemberJoined) => new MemberJoined(e.AggregateId, JsonSerializer.Deserialize<MemberJoinedData>(e.Data), e.DateTime, e.Sequence, e.UserId, e.Version),
                nameof(MemberRemoved) => new MemberRemoved(e.AggregateId, JsonSerializer.Deserialize<MemberRemovedData>(e.Data), e.DateTime, e.Sequence, e.UserId, e.Version),
                nameof(PermissionChanged) => new PermissionChanged(e.AggregateId, JsonSerializer.Deserialize<PermissionChangedData>(e.Data), e.DateTime, e.Sequence, e.UserId, e.Version),
                nameof(MemberLeft) => new MemberLeft(e.AggregateId, JsonSerializer.Deserialize<MemberLeftData>(e.Data), e.DateTime, e.Sequence, e.UserId, e.Version),
                _ => throw new Exception("Type not defined Exception"),
            };
        }
    }
}
