using SubscriptionCommand.Events;

namespace SubscriptionCommand.Domain
{
    public class Outbox
    {
        public Outbox()
        {
            
        }
        public Outbox(EventEntity @event)
        {
            Event = @event;
        }
        public int Id{ get; set; }
        public EventEntity? Event{ get; set; }
    }
}
