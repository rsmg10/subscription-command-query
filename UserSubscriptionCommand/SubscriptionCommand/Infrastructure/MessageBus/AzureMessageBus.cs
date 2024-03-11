using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using SubscriptionCommand.Infrastructure.Presistance;

namespace SubscriptionCommand.Infrastructure.MessageBus
{
    public class AzureMessageBus
    {
        private readonly IServiceProvider _provider;
        private readonly object _lock = new();
        private readonly ServiceBusSender? _sender;
        private bool IsBusy { get; set; }
        public AzureMessageBus(IServiceProvider provider, ServiceBusClient client)
        {
            _provider = provider;
            _sender = client.CreateSender("radwan-user-subscription-command-2") ;
        }


        public void Publish()
        {
            Task.Run(() =>
            {
                try
                {

                    lock (_lock)
                    {
                        if (IsBusy) return;

                        PublishMessages().GetAwaiter().GetResult();
                    }
                }
                catch(Exception e)
                {

                }
                finally
                {
                    IsBusy = false;
                }
            });
        }

        private async Task PublishMessages()
        {
            while (true)
            {
                using var scope = _provider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDatabase>();

                var messages = await dbContext.Outbox
                    .Include(x => x.Event)
                    .OrderBy(x => x.Id)
                    .Take(100)
                    .ToListAsync();

                if (messages.Count == 0) return;

                foreach (var message in messages)
                {
                    if (message.Event is null)
                    {
                        throw new InvalidOperationException("Event is null, please include the event in the query");
                    }

                    var json = JsonSerializer.Serialize(message.Event);

                    var serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(json))
                    {
                        PartitionKey = message.Event.AggregateId.ToString(),
                        SessionId = message.Event.AggregateId.ToString(),
                        Subject = message.Event.Type
                    };
                     
                    await _sender.SendMessageAsync(serviceBusMessage);

                    dbContext.Outbox.Remove(message);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
