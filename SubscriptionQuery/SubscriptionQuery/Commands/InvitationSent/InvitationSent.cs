using SubscriptionQuery.Domain.Enums;
using SubscriptionQuery.Events;

namespace SubscriptionQuery.Commands.InvitationSent
{
    public record InvitationSent(
            Guid AggregateId,
            InvitationSentData? Data,
            DateTime DateTime,
            int Sequence,
            string UserId,
            int Version
        ) : Event<InvitationSentData>(AggregateId: AggregateId, Data: Data, DateTime: DateTime, Sequence: Sequence, UserId: UserId, Version: Version);

    public record InvitationSentData(Guid UserId, Guid SubscriptionId, Guid MemberId, Permissions Permission, Guid InvitationId, Guid AccountId);
    

}
