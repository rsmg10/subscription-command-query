using MediatR;
using SubscriptionCommandProto;

namespace SubscriptionCommand.Commands.AcceptInvitation
{
    public record AcceptInvitationCommand(Guid AccountId, Guid MemberId, Guid SubscriptionId, Guid UserId) : IRequest<Response>;
}
