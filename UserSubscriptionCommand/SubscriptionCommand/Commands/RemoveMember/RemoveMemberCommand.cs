using MediatR;
using SubscriptionCommandProto;

namespace SubscriptionCommand.Commands.RemoveMember
{
    public record RemoveMemberCommand(Guid AccountId, Guid SubscriptionId, Guid MemberId, Guid UserId) : IRequest<Response>;

}
