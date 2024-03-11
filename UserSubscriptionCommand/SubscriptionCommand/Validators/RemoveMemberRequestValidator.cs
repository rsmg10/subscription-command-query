using FluentValidation;
using FluentValidation.Validators;
using SubscriptionCommandProto;

public class RemoveMemberRequestValidator : AbstractValidator<RemoveMemberRequest>
{

    public RemoveMemberRequestValidator()
    {
        RuleFor(x => x.AccountId).NotNull().NotEmpty().Must(x=> Guid.TryParse(x, out _));
        RuleFor(x => x.SubscriptionId).NotNull().NotEmpty().Must(x => Guid.TryParse(x, out _));
        RuleFor(x => x.MemberId).NotNull().NotEmpty().Must(x => Guid.TryParse(x, out _)); 
        RuleFor(x => x.UserId).NotNull().NotEmpty().Must(x => Guid.TryParse(x, out _)); 
     }
  
}
 