using MediatR;
using SubscriptionCommand.Abstraction;
using SubscriptionCommand.Domain;
using SubscriptionCommand.Exceptions;
using SubscriptionCommand.Extensions;
using SubscriptionCommandProto;

namespace SubscriptionCommand.Commands.SendInvitation
{
    public class SendInvitationCommandHandler : IRequestHandler<SendInvitationCommand, Response>
    {
        private readonly IEventStore _eventStore;

        public SendInvitationCommandHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<Response> Handle(SendInvitationCommand request, CancellationToken cancellationToken)
        {
            // validate subscriptionId and that it is subscription is particular type
            // either call command side or listen to event
            // validate UserId

            if (request.UserId == request.MemberId)
                throw new BusinessRuleViolationException("cant send invitation to same user");

            var events = await _eventStore.GetAllAsync(GuidExtensions.CombineGuids(request.SubscriptionId, request.MemberId), cancellationToken);

            bool isNew = (!events.Any() || events is null);

            var userSubscription = isNew ?
                 new UserSubscription(request.SubscriptionId, request.MemberId) :
                UserSubscription.LoadFromHistory(events!);

            userSubscription.SendInvitation(request);
            await _eventStore.CommitAsync(userSubscription, cancellationToken);

            return new Response
            {
                Id = request.MemberId.ToString(),
                Message = "Invitation Sent"
            };
        }
    }
}
