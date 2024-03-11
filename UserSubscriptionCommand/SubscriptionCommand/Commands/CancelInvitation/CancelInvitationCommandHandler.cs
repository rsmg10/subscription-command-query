using MediatR;
using SubscriptionCommand.Abstraction;
using SubscriptionCommand.Domain;
using SubscriptionCommand.Exceptions;
using SubscriptionCommand.Extensions; 
using SubscriptionCommandProto;

namespace SubscriptionCommand.Commands.CancelInvitation
{
    public class CancelInvitationCommandHandler : IRequestHandler<CancelInvitationCommand, Response>
    {
        
        private readonly IEventStore _eventStore;

        public CancelInvitationCommandHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<Response> Handle(CancelInvitationCommand request, CancellationToken cancellationToken)
        {
            // validation
            // todo: validate account

            if (request.UserId == request.MemberId)
                throw new BusinessRuleViolationException("cant send invitation to same user");

            var events = await _eventStore.GetAllAsync(GuidExtensions.CombineGuids(request.SubscriptionId, request.MemberId), cancellationToken);

            if (!events.Any() || events is null)
                throw new BusinessRuleViolationException("invalid Id");
            
            var userSubscription = UserSubscription.LoadFromHistory(events);
            
            userSubscription.CancelInvitation(request);
            await _eventStore.CommitAsync(userSubscription, cancellationToken);

            return new Response()
            {
                Id = userSubscription.Id.ToString(),
                Message = "Invitation Cancelled Successfully"
            };
            
            
        }
    }
}
