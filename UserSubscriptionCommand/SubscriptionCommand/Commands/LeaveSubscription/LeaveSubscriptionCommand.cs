using MediatR;
using SubscriptionCommandProto;

namespace SubscriptionCommand.Commands.LeaveSubscription
{
    public record LeaveSubscriptionCommand(Guid AccountId, Guid SubscriptionId, Guid MemberId) : IRequest<Response>; 
}
