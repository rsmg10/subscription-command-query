using MediatR;
using SubscriptionCommand.Domain.Enums;
using SubscriptionCommandProto;

namespace SubscriptionCommand.Commands.SendInvitation
{
    public record SendInvitationCommand(Guid UserId, Guid MemberId, Guid SubscriptionId , Permissions Permission, Guid AccountId ) : IRequest<Response>;
}
  