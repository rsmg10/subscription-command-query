namespace SubscriptionCommand.Events
{
    public record InvitationCancelled(
            Guid AggregateId,
            InvitationCancelledData? Data,
            DateTime DateTime,
            int Sequence,
            string UserId,
            int Version
        ) : Event<InvitationCancelledData>(AggregateId: AggregateId, Data: Data, DateTime: DateTime, Sequence: Sequence, UserId: UserId, Version: Version);

    public record InvitationCancelledData();
}
