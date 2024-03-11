using MediatR;
using SubscriptionCommandProto;

namespace SubscriptionCommand.Commands.RejectInvitation
{
    public record RejectInvitationCommand(Guid AccountId, Guid SubscriptionId, Guid UserId, Guid MemberId) : IRequest<Response>;
}
