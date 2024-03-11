
using SubscriptionQuery.Domain.Enums;
using SubscriptionQuery.Events;

namespace SubscriptionQuery.Commands.PermissionChanged
{
    public record PermissionChanged
        (
            Guid AggregateId,
            PermissionChangedData? Data,
            DateTime DateTime,
            int Sequence,
            string UserId,
            int Version
        ) : Event<PermissionChangedData>(AggregateId: AggregateId, Data: Data, DateTime: DateTime, Sequence: Sequence, UserId: UserId, Version: Version);

    public record PermissionChangedData(Guid AccountId, Guid UserId, Guid MemberId, Guid SubscriptionId, Permissions Permission);
}
