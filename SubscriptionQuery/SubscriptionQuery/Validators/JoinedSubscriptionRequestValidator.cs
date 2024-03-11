using FluentValidation;
using SubscriptionQuery;

namespace SubscriptionQuery.Validators
{
    public class JoinedSubscriptionRequestValidator : AbstractValidator<JoinedSubscriptionRequest>
    {
        public JoinedSubscriptionRequestValidator()
        {
            RuleFor(r => r.UserId)
                .Must(id => Guid.TryParse(id, out _));
        }
    }
 
}
