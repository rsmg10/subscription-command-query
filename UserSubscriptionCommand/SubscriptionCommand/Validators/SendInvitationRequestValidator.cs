using FluentValidation;
using SubscriptionCommandProto;

namespace SubscriptionCommand.Validators;

public class SendInvitationRequestValidator : AbstractValidator<SendInvitationRequest>
{
    public SendInvitationRequestValidator()
    {
        RuleFor(x => x.AccountId).NotEmpty().NotNull();
        RuleFor(x => x.MemberId).NotEmpty().NotNull().Must(((request, s) => request.MemberId!= request.UserId)).WithMessage("user Id must not be equal to member Id");
        RuleFor(x => x.Permission).NotEmpty().NotNull().GreaterThan(0);
        RuleFor(x => x.SubscriptionId).NotEmpty().NotNull();
        RuleFor(x => x.UserId).NotEmpty().NotNull();
    }
    
}