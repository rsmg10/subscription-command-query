using SubscriptionQuery.Domain.Enums;
using SubscriptionQuery.Events;

namespace SubscriptionQuery.Commands.MemberJoined
{
    public record MemberJoined (
            Guid AggregateId,
            MemberJoinedData? Data,
            DateTime DateTime,
            int Sequence,
            string UserId,
            int Version
        ) : Event<MemberJoinedData>(AggregateId: AggregateId, Data: Data, DateTime: DateTime, Sequence: Sequence, UserId: UserId, Version: Version);
    public record MemberJoinedData(Guid AccountId, Guid UserId, Guid MemberId, Guid SubscriptionId, Permissions Permission, JoinedBy JoinedBy);
}
