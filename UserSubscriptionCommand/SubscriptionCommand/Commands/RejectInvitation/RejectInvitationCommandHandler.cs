using MediatR;
using SubscriptionCommand.Abstraction;
using SubscriptionCommand.Domain;
using SubscriptionCommand.Exceptions;
using SubscriptionCommand.Extensions;
using SubscriptionCommandProto;

namespace SubscriptionCommand.Commands.RejectInvitation
{
    public record RejectInvitationCommandHandler : IRequestHandler<RejectInvitationCommand, Response>
    {

        private readonly IEventStore _eventStore;

        public RejectInvitationCommandHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<Response> Handle(RejectInvitationCommand request, CancellationToken cancellationToken)
        {
            // validation
            // todo: validate account

            if (request.UserId == request.MemberId)
                throw new BusinessRuleViolationException("cant send invitation to same user");

            var events = await _eventStore.GetAllAsync(GuidExtensions.CombineGuids(request.SubscriptionId, request.MemberId), cancellationToken);

            if (!events.Any() || events is null)
                throw new BusinessRuleViolationException("invalid subscription id or member id");


            var userSubscription = UserSubscription.LoadFromHistory(events);

            userSubscription.RejectInvitation(request);

            await _eventStore.CommitAsync(userSubscription, cancellationToken);
            return new Response()
            {
                Id = userSubscription.Id.ToString(),
                Message = "Invitation Rejected Successfully"
            };
        }
    }
}
