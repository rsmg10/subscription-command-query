using MediatR;
using SubscriptionCommand.Domain.Enums;
using SubscriptionCommandProto;

namespace SubscriptionCommand.Commands.ChangePermission
{
    public record ChangePermissionCommand(Guid AccountId, Guid SubscriptionId, Guid MemberId, Guid UserId, Permissions Permission) : IRequest<Response>;
}
 