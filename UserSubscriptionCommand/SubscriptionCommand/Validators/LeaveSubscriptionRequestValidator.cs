using FluentValidation;
using FluentValidation.Validators;
using SubscriptionCommandProto;

public class LeaveSubscriptionRequestValidator : AbstractValidator<LeaveRequest>
{

    public LeaveSubscriptionRequestValidator()
    {
        RuleFor(x => x.AccountId).NotNull().NotEmpty().Must(x=> Guid.TryParse(x, out _));
        RuleFor(x => x.SubscriptionId).NotNull().NotEmpty().Must(x => Guid.TryParse(x, out _));
        RuleFor(x => x.MemberId).NotNull().NotEmpty().Must(x => Guid.TryParse(x, out _)); 
    }
  
}
 