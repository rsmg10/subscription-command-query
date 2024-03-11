using SubscriptionCommand.Domain.Enums;

namespace SubscriptionCommand.Events
{
    public record MemberRemoved(
            Guid AggregateId,
            MemberRemovedData? Data,
            DateTime DateTime,
            int Sequence,
            string UserId,
            int Version
        ) : Event<MemberRemovedData>(AggregateId: AggregateId, Data: Data, DateTime: DateTime, Sequence: Sequence, UserId: UserId, Version: Version);

    public record MemberRemovedData(Guid UserId, Guid SubscriptionId, Guid MemberId);

}
