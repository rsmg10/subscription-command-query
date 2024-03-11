namespace SubscriptionCommand.Events
{
    public record InvitationAccepted
        (
            Guid AggregateId,
            InvitationAcceptedData? Data,
            DateTime DateTime,
            int Sequence,
            string UserId,
            int Version
        ) : Event<InvitationAcceptedData>(AggregateId: AggregateId, Data: Data, DateTime: DateTime, Sequence: Sequence, UserId: UserId, Version: Version);

    public record InvitationAcceptedData();
}
