using Azure.Core;
using SubscriptionCommand.Domain.Enums;

namespace SubscriptionCommand.Events
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
