using SubscriptionCommand.Commands.AcceptInvitation;
using SubscriptionCommand.Commands.CancelInvitation;
using SubscriptionCommand.Commands.ChangePermission;
using SubscriptionCommand.Commands.JoinMember;
using SubscriptionCommand.Commands.LeaveSubscription;
using SubscriptionCommand.Commands.RejectInvitation;
using SubscriptionCommand.Commands.RemoveMember;
using SubscriptionCommand.Commands.SendInvitation;
using SubscriptionCommand.Domain.Enums;
using SubscriptionCommandProto;

namespace SubscriptionCommand.Extensions
{
    public static class CommandsExtensions
    { 
        public static AcceptInvitationCommand ToCommand(this AcceptInvitationRequest request)
        {
            return new AcceptInvitationCommand(request.AccountId.ToGuid(),
                request.MemberId.ToGuid(), 
                request.SubscriptionId.ToGuid(),
                request.UserId.ToGuid());
        }

        public static RejectInvitationCommand ToCommand(this RejectInvitationRequest request)
        {
            return new RejectInvitationCommand(request.AccountId.ToGuid(),
                request.SubscriptionId.ToGuid(), 
                request.UserId.ToGuid(), 
                request.MemberId.ToGuid());
        }

        public static CancelInvitationCommand ToCommand(this CancelInvitationRequest request)
        {
            return new CancelInvitationCommand(request.AccountId.ToGuid(), 
                request.SubscriptionId.ToGuid(),
                request.UserId.ToGuid(), 
                request.MemberId.ToGuid());
        }

        public static SendInvitationCommand ToCommand(this SendInvitationRequest request)
        {
            var s = Guid.NewGuid().ToString();
            return new SendInvitationCommand(request.UserId.ToGuid(),
                request.MemberId.ToGuid(),
                request.SubscriptionId.ToGuid(),
                (Permissions) request.Permission,
                request.AccountId.ToGuid()); ;
        } 


        public static RemoveMemberCommand ToCommand(this RemoveMemberRequest request)
        {
            return new RemoveMemberCommand(request.AccountId.ToGuid(),
                request.SubscriptionId.ToGuid(),
                request.MemberId.ToGuid(),
                request.UserId.ToGuid()); ;
        } 
        public static JoinMemberCommand ToCommand(this JoinMemberRequest request)
        {
            return new JoinMemberCommand(request.AccountId.ToGuid(),
                request.SubscriptionId.ToGuid(),
                request.UserId.ToGuid(),
                request.MemberId.ToGuid(),
                (Permissions)request.Permission);
        }
        public static ChangePermissionCommand ToCommand(this ChangePermissionRequest request)
        {
            return new ChangePermissionCommand(request.AccountId.ToGuid(),
                    request.SubscriptionId.ToGuid(),
                    request.MemberId.ToGuid(),
                    request.UserId.ToGuid(),
                    (Permissions)request.Permission); 
        }
        public static LeaveSubscriptionCommand ToCommand(this LeaveRequest request)
        {
            return new LeaveSubscriptionCommand(request.AccountId.ToGuid(),
                          request.SubscriptionId.ToGuid(),
                          request.MemberId.ToGuid());
        }

        public static Guid ToGuid(this string guid) => Guid.Parse(guid);
    }
}
