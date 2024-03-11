using FluentValidation;
using SubscriptionQuery;

namespace SubscriptionQuery.Validators
{
    public class PendingInvitationsReceivedRequestValidator : AbstractValidator<PendingInvitationsReceivedRequest>
    {
        public PendingInvitationsReceivedRequestValidator()
        {
            RuleFor(r => r.UserId)
                .Must(id => Guid.TryParse(id, out _)); 
        }
    }
 
}
