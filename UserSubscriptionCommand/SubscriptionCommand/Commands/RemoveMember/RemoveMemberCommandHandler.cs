using MediatR;
using SubscriptionCommand.Abstraction;
using SubscriptionCommand.Domain;
using SubscriptionCommand.Extensions;
using SubscriptionCommandProto;

namespace SubscriptionCommand.Commands.RemoveMember
{
    public record RemoveMemberCommandHandler : IRequestHandler<RemoveMemberCommand, Response>
    {
        private readonly IEventStore _eventStore;

        public RemoveMemberCommandHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<Response> Handle(RemoveMemberCommand command, CancellationToken cancellationToken)
        {

            // validate account, subscription, that user is ownder or admin

            var events = await _eventStore.GetAllAsync(GuidExtensions.CombineGuids(command.SubscriptionId, command.MemberId), cancellationToken);

            var subscriptionAggregate = UserSubscription.LoadFromHistory(events);

            subscriptionAggregate.RemoveMember(command);

            await _eventStore.CommitAsync(subscriptionAggregate, cancellationToken);
            return new Response
            {
                Id = command.UserId.ToString(),
                Message = "Member Removed Successfully"
            };
        }
    }
}
