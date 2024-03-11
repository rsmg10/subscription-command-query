using Azure.Core;
using MediatR;
using SubscriptionCommand.Abstraction;
using SubscriptionCommand.Domain;
using SubscriptionCommand.Domain.Enums;
using SubscriptionCommand.Exceptions;
using SubscriptionCommand.Extensions;
using SubscriptionCommandProto;

namespace SubscriptionCommand.Commands.ChangePermission
{
    public class ChangePermissionCommandHandler : IRequestHandler<ChangePermissionCommand, Response>
    {
        private readonly IEventStore _eventStore;

        public ChangePermissionCommandHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }
        public async Task<Response> Handle(ChangePermissionCommand command, CancellationToken cancellationToken)
        {
            // validate account
            // validate subscription
            // validate that userId is admin or subscription owner

            var aggregateId = GuidExtensions.CombineGuids(command.SubscriptionId, command.MemberId);
            var events = await _eventStore.GetAllAsync(aggregateId, cancellationToken);
            if(!events.Any() ) {
                throw new NotFoundException("aggregate id not found");
            }
            var subscriptionAggregate = UserSubscription.LoadFromHistory(events);
            
            subscriptionAggregate.ChangePermission(command);
            await _eventStore.CommitAsync(subscriptionAggregate, cancellationToken);

            return new Response()
            {
                Id = command.UserId.ToString(),
                Message = "Permission changed"
            };

        }
    }
}
 