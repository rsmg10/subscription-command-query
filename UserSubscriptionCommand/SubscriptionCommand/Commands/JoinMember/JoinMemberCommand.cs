using MediatR;
using SubscriptionCommand.Domain.Enums;
using SubscriptionCommandProto;

namespace SubscriptionCommand.Commands.JoinMember
{
    public record JoinMemberCommand(Guid AccountId , Guid SubscriptionId , Guid UserId , Guid MemberId, Permissions Permission) : IRequest<Response>;
}
