using System.ComponentModel.DataAnnotations.Schema;
using MediatR;
using Newtonsoft.Json;

namespace SubscriptionQuery.Events
{
    public class EventEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid AggregateId { get; set; }
        public string Type { get; set; }
        public string? Data { get; set; } 
        public DateTime DateTime { get; set; }
        public string UserId{ get; set; }
        public int Version { get; set; }
        public int Sequence  { get; set; }


    }
    public record Event(
        int Id, 
        Guid AggregateId,   
        DateTime DateTime,
        int Sequence, 
        string UserId,
        int Version
        ) : IRequest<bool>,IEvent
    {
        public string Type => GetType().Name;
        dynamic IEvent.Data => ((dynamic)this).Data; // service bus
    }

    public record Event<T>(
    Guid AggregateId,
    T Data,
    DateTime DateTime,
    int Sequence,
    string UserId, 
    int Version
    ) : Event( Id: default, AggregateId: AggregateId, DateTime: DateTime, Sequence: Sequence, UserId: UserId, Version: Version)
    {
        public string GetDataAsJson() =>
        JsonConvert.SerializeObject(Data, new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
        });
    }
 
    public interface IEvent
    {
        public dynamic Data { get; }
    }
}
