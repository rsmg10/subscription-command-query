namespace SubscriptionCommand.Events
{
    public record MemberLeft(
           Guid AggregateId,
           MemberLeftData? Data,
           DateTime DateTime,
           int Sequence,
           string UserId,
           int Version
        ) : Event<MemberLeftData>(AggregateId: AggregateId, Data: Data, DateTime: DateTime, Sequence: Sequence, UserId: UserId, Version: Version);



    public record MemberLeftData(Guid accountId, Guid memberId, Guid subscriptionId);
}