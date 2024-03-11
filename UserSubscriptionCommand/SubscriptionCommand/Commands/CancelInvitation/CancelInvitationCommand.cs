using MediatR;
using SubscriptionCommandProto;

namespace SubscriptionCommand.Commands.CancelInvitation
{
    public record CancelInvitationCommand(Guid AccountId, Guid SubscriptionId, Guid UserId, Guid MemberId) 
        : IRequest<Response>;

}
