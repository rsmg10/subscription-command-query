 
using MediatR;
using SubscriptionCommand.Abstraction;
using SubscriptionCommand.Domain;
using SubscriptionCommand.Extensions;
using SubscriptionCommandProto;

namespace SubscriptionCommand.Commands.JoinMember
{
    public class JoinMemberCommandHandler : IRequestHandler<JoinMemberCommand, Response>
    {
        private readonly IEventStore _eventStore;
        public JoinMemberCommandHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }
        public async Task<Response> Handle(JoinMemberCommand request, CancellationToken cancellationToken)
        {
            // validate account
            // validate subscription 
            // validate user is admin

            var events = await _eventStore.GetAllAsync(GuidExtensions.CombineGuids(request.SubscriptionId, request.MemberId), cancellationToken);


            bool isNew = (!events.Any() || events is null);

            var subscriptionAggregate = isNew ?
                 new UserSubscription(request.SubscriptionId, request.MemberId) :
                UserSubscription.LoadFromHistory(events!);


            subscriptionAggregate = UserSubscription.LoadFromHistory(events!);
            subscriptionAggregate.JoinMember(request);
            await _eventStore.CommitAsync(subscriptionAggregate, cancellationToken);

            return new Response
            {
                Id = request.UserId.ToString(),
                Message = "user joined successfully"
            };
        }
    }
}
