using SubscriptionCommand.Abstraction;
using SubscriptionCommand.Events;

namespace SubscriptionCommand.Domain
{
    public abstract class Aggregate<T> where T : Aggregate<T>, IAggregate
    {
        private readonly List<Event> _uncommittedEvents = new List<Event>();
        public Aggregate() { }
        public Guid Id { get; set; }

        public int Sequence { get; set; }

        public IReadOnlyList<Event> GetUncommittedEvents()
        {
            return _uncommittedEvents;
        }

        public void MarkChangesAsCommitted()
        {
            _uncommittedEvents.Clear();
        }

        public static T LoadFromHistory(List<Event> history)
        {
            var instance = CreateNewInstance();
            instance.ApplyPreviouslyCommittedChanges(history);
            return instance;
        }

        private void ApplyPreviouslyCommittedChanges(List<Event> events)
        {
            foreach (var @event in events)
            { 
                ValidateAndApplyChange(@event);
            }
        }
        private void SetIdAndSequence(Event @event)
        {
            if (@event.Sequence == 1)
                Id = @event.AggregateId;

        }
        private void ValidateEvent(Event @event)
        {
        }
        private void ValidateAndApplyChange(Event @event)
        {
            SetIdAndSequence(@event);
            ValidateEvent(@event);
            Mutate(@event);
        }
        private static T CreateNewInstance()
        {
            var instance = (T?)Activator.CreateInstance(typeof(T), nonPublic: true);
            if (instance == null) throw new Exception();
            return instance;
        }
        protected abstract void Mutate(Event @event);
        protected void ApplyNewChange(Event @event)
        {
            ValidateAndApplyChange(@event); 
            _uncommittedEvents.Add(@event);
        }
    }
}
