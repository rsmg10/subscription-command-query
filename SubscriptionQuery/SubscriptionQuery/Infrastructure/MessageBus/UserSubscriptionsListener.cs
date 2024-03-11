using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using MediatR;
using SubscriptionQuery.Commands.InvitationAccepted;
using SubscriptionQuery.Commands.InvitationCancelled;
using SubscriptionQuery.Commands.InvitationRejected;
using SubscriptionQuery.Commands.InvitationSent;
using SubscriptionQuery.Commands.MemberJoined;
using SubscriptionQuery.Commands.MemberLeft;
using SubscriptionQuery.Commands.MemberRemoved;
using SubscriptionQuery.Commands.PermissionChanged;
using SubscriptionQuery.Events;
using InvitationAccepted = SubscriptionQuery.Commands.InvitationAccepted.InvitationAccepted;

namespace SubscriptionQuery.Infrastructure.MessageBus
{
    public class UserSubscriptionsListener : IHostedService
    {
        private readonly ServiceBusSessionProcessor _processor;
        private readonly ILogger<UserSubscriptionsListener> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ServiceBusProcessor _deadLetterProcessor;

        public UserSubscriptionsListener(ServiceBusClient serviceBus, IConfiguration configuration, ILogger<UserSubscriptionsListener> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _processor = serviceBus.CreateSessionProcessor(
                topicName: "radwan-user-subscription-command-2",
                subscriptionName: "invitations2",
                options: new ServiceBusSessionProcessorOptions()
                {
                    PrefetchCount = 1,
                    MaxConcurrentSessions = 100,
                    MaxConcurrentCallsPerSession = 1
                });

            _processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
            _processor.ProcessErrorAsync += Processor_ProcessErrorAsync;

            _deadLetterProcessor = serviceBus.CreateProcessor(
                topicName: "radwan-user-subscription-command-2",
                subscriptionName: "invitations2",
                options: new ServiceBusProcessorOptions()
                {
                    AutoCompleteMessages = false,
                    PrefetchCount = 10,
                    MaxConcurrentCalls = 10,
                    SubQueue = SubQueue.DeadLetter
                });

            _deadLetterProcessor.ProcessMessageAsync += DeadLetterProcessor_ProcessMessageAsync;
            _deadLetterProcessor.ProcessErrorAsync += Processor_ProcessErrorAsync;
        }

        private async Task DeadLetterProcessor_ProcessMessageAsync(ProcessMessageEventArgs arg)
        {
        }

        private async Task Processor_ProcessMessageAsync(ProcessSessionMessageEventArgs arg)
        {
            try
            {
                //await arg.CompleteMessageAsync(arg.Message);
                var json = Encoding.UTF8.GetString(arg.Message.Body);

                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var e = Deserialize<EventEntity>(json);  

                var isHandled = arg.Message.Subject switch
                {
                    nameof(InvitationAccepted) => await mediator.Send(new InvitationAccepted(e.AggregateId, JsonSerializer.Deserialize<InvitationAcceptedData>(e.Data), e.DateTime, e.Sequence, e.UserId, e.Version)),
                    nameof(InvitationCancelled) => await mediator.Send(new InvitationCancelled(e.AggregateId, JsonSerializer.Deserialize<InvitationCancelledData>(e.Data), e.DateTime, e.Sequence, e.UserId, e.Version)),
                    nameof(InvitationRejected) => await mediator.Send(new InvitationRejected(e.AggregateId, JsonSerializer.Deserialize<InvitationRejectedData>(e.Data), e.DateTime, e.Sequence, e.UserId, e.Version)),
                    nameof(InvitationSent) => await mediator.Send(new InvitationSent(e.AggregateId, JsonSerializer.Deserialize<InvitationSentData>(e.Data), e.DateTime, e.Sequence, e.UserId, e.Version)),
                    nameof(PermissionChanged) => await mediator.Send(new PermissionChanged(e.AggregateId, JsonSerializer.Deserialize<PermissionChangedData>(e.Data), e.DateTime, e.Sequence, e.UserId, e.Version)),
                    nameof(MemberJoined) => await mediator.Send(new MemberJoined(e.AggregateId, JsonSerializer.Deserialize<MemberJoinedData>(e.Data), e.DateTime, e.Sequence, e.UserId, e.Version)),
                    nameof(MemberRemoved) => await mediator.Send(new MemberRemoved(e.AggregateId, JsonSerializer.Deserialize<MemberRemovedData>(e.Data), e.DateTime, e.Sequence, e.UserId, e.Version)),
                    nameof(MemberLeft) => await mediator.Send(new MemberLeft(e.AggregateId, JsonSerializer.Deserialize<MemberLeftData>(e.Data), e.DateTime, e.Sequence, e.UserId, e.Version)),
                    _ => await mediator.Send(Deserialize<UnknownEvent>(json)),
                };

                if (isHandled)
                {
                    await arg.CompleteMessageAsync(arg.Message);
                }
                else
                {
                    _logger.LogWarning("Message {MessageId} not handled", arg.Message.MessageId);
                    await Task.Delay(5000);
                    await arg.AbandonMessageAsync(arg.Message);
                }
            }
            catch (Exception e)
            {

                throw;
            }
          
        }


        private static T Deserialize<T>(string json)
            => JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException("Failed to deserialize message");

        private Task Processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            _logger.LogCritical(arg.Exception, "Message handler encountered an exception," +
                " Error Source:{ErrorSource}," +
                " Entity Path:{EntityPath}",
                arg.ErrorSource.ToString(),
                arg.EntityPath
            ) ;

            return Task.CompletedTask;
        }

        public Task StartAsync(CancellationToken cancellationToken) => _processor.StartProcessingAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken) => _processor.CloseAsync(cancellationToken);
    }
    public class EventEntity
    { 
        public int Id { get; set; }
        public Guid AggregateId { get; set; }
        public string Type { get; set; }
        public string? Data { get; set; }
        public DateTime DateTime { get; set; }
        public string UserId { get; set; }
        public int Version { get; set; }
        public int Sequence { get; set; }


    }
}
