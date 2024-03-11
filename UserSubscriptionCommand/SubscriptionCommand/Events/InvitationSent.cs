using SubscriptionCommand.Domain.Enums;

namespace SubscriptionCommand.Events
{
    public record InvitationSent(
            Guid AggregateId,
            InvitationSentData? Data,
            DateTime DateTime,
            int Sequence,
            string UserId,
            int Version
        ) : Event<InvitationSentData>(AggregateId: AggregateId, Data: Data, DateTime: DateTime, Sequence: Sequence, UserId: UserId, Version: Version);

    public record InvitationSentData(Guid UserId, Guid SubscriptionId, Guid MemberId,  Permissions Permission, Guid InvitationId, Guid AccountId);

}
