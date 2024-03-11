using SubscriptionQuery.Events;

namespace SubscriptionQuery.Commands.InvitationRejected
{

    public record InvitationRejected(
            Guid AggregateId,
            InvitationRejectedData? Data,
            DateTime DateTime,
            int Sequence,
            string UserId,
            int Version
        ) : Event<InvitationRejectedData?>(AggregateId: AggregateId, Data: Data, DateTime: DateTime, Sequence: Sequence, UserId: UserId, Version: Version);

     public record InvitationRejectedData(Guid InvitationId);




}
