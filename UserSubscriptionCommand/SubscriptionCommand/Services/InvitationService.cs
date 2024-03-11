using Grpc.Core;
using MediatR;
using SubscriptionCommand.Extensions;
using SubscriptionCommandProto;

namespace SubscriptionCommand.Services;

public class InvitationService : SubscriptionCommandProto.SubscriptionCommand.SubscriptionCommandBase
{
    private readonly ILogger<InvitationService> _logger;
    private readonly IMediator _mediator;

    public InvitationService(ILogger<InvitationService> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }


    public override async Task<Response> SendInvitation(SendInvitationRequest request, ServerCallContext context)
    {
            var result = await _mediator.Send(request.ToCommand());
            return result;
    }
    public override async Task<Response> AcceptInvitation(AcceptInvitationRequest request, ServerCallContext context)
    { 
        var result = await _mediator.Send(request.ToCommand());
        return result;
    }
    public override async Task<Response> CancelInvitation(CancelInvitationRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(request.ToCommand());
        return result;
    }
    public override async Task<Response> RejectInvitation(RejectInvitationRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(request.ToCommand());
        return result;
    }


    // member uses this to leave subscription
    public override async Task<Response> Leave(LeaveRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(request.ToCommand());
        return result;
    }

    public override async Task<Response> ChangePermission(ChangePermissionRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(request.ToCommand());
        return result;
    }

    // admin uses this to add member to subscription
    public override async Task<Response> JoinMember(JoinMemberRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(request.ToCommand());
        return result;
    }

    // admin uses this to remove member from subscription
    public override async Task<Response> RemoveMember(RemoveMemberRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(request.ToCommand());
        return result;
    } 



}