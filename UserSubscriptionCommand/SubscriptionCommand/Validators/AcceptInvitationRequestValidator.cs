using FluentValidation;
using SubscriptionCommandProto;

namespace SubscriptionCommand.Validators;

public class AcceptInvitationRequestValidator : AbstractValidator<AcceptInvitationRequest>
{
    public AcceptInvitationRequestValidator()
    {
        RuleFor(x => x.AccountId).NotEmpty().NotNull();
        RuleFor(x => x.MemberId).NotEmpty().NotNull(); 
        RuleFor(x => x.SubscriptionId).NotEmpty().NotNull();
        RuleFor(x => x.UserId).NotEmpty().NotNull();
    }
    
}