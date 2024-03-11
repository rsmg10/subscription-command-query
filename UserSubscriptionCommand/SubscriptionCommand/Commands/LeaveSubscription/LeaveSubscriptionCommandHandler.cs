using MediatR;
using SubscriptionCommand.Abstraction;
using SubscriptionCommand.Domain;
using SubscriptionCommand.Extensions;
using SubscriptionCommandProto;

namespace SubscriptionCommand.Commands.LeaveSubscription
{
    public class LeaveSubscriptionCommandHandler : IRequestHandler<LeaveSubscriptionCommand, Response>
    {
        private readonly IEventStore _eventStore;

        public LeaveSubscriptionCommandHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<Response> Handle(LeaveSubscriptionCommand command, CancellationToken cancellationToken)
        {
            // validate account and subscription 

            var events = await _eventStore.GetAllAsync(GuidExtensions.CombineGuids(command.SubscriptionId, command.MemberId), cancellationToken);

            var subscriptionAggregate = UserSubscription.LoadFromHistory(events);

            subscriptionAggregate.Leave(command);

            await _eventStore.CommitAsync(subscriptionAggregate, cancellationToken);

            return new Response
            {
                Id = command.MemberId.ToString(),
                Message = "user left successfully"
            };

        }
    } 
}
