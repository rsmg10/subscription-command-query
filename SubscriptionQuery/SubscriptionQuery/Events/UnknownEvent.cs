namespace SubscriptionQuery.Events;

public record UnknownEvent(Guid AggregateId, object Data, DateTime DateTime, int Sequence, string UserId, int Version) : Event<object>(AggregateId, Data, DateTime, Sequence, UserId, Version);