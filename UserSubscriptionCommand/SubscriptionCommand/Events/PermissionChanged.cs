using SubscriptionCommand.Domain.Enums;

namespace SubscriptionCommand.Events
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
