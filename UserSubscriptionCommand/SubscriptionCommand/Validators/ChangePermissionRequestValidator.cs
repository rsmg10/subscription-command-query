using FluentValidation;
using FluentValidation.Validators;
using SubscriptionCommand.Domain.Enums;
using SubscriptionCommandProto;

public class ChangePermissionRequestValidator : AbstractValidator<ChangePermissionRequest>
{

    public ChangePermissionRequestValidator()
    {
        RuleFor(x => x.AccountId).NotNull().NotEmpty().Must(x=> Guid.TryParse(x, out _));
        RuleFor(x => x.SubscriptionId).NotNull().NotEmpty().Must(x => Guid.TryParse(x, out _));
        RuleFor(x => x.MemberId).NotNull().NotEmpty().Must(x => Guid.TryParse(x, out _)); 
        RuleFor(x => x.UserId).NotNull().NotEmpty().Must(x => Guid.TryParse(x, out _)); 
        RuleFor(x => (Permissions)x.Permission).IsInEnum(); 
    }
}

 