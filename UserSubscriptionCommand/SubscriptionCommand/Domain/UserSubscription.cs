using Microsoft.AspNetCore.Mvc.ModelBinding;
using SubscriptionCommand.Commands.SendInvitation;
using SubscriptionCommand.Domain.Enums;
using SubscriptionCommand.Events;
using System.Security.Principal;
using SubscriptionCommand.Abstraction;
using SubscriptionCommand.Commands.AcceptInvitation;
using SubscriptionCommand.Commands.CancelInvitation;
using SubscriptionCommand.Commands.RejectInvitation;
using SubscriptionCommand.Exceptions;
using SubscriptionCommand.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SubscriptionCommand.Commands.ChangePermission;
using SubscriptionCommand.Commands.JoinMember;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using SubscriptionCommand.Commands.LeaveSubscription;
using SubscriptionCommand.Commands.RemoveMember;
using Azure.Core;

namespace SubscriptionCommand.Domain
{
    public class UserSubscription : Aggregate<UserSubscription>, IAggregate
    {
        public UserSubscription(Guid subscriptionId, Guid memberId) 
        {
            Id = GuidExtensions.CombineGuids(subscriptionId, memberId);
        }
        public UserSubscription()
        {
            
        }
        public Guid OwnerId { get; set; }
        public Guid MemberId { get; set; }
        public SubscriptionType Type { get; set; }
        public Permissions Permission { get; set; }
        public List<Invitation> Invitations { get; set; } = new List<Invitation>();
        public bool IsJoined { get; set; }

 
        public void RejectInvitation(RejectInvitationCommand command)
        {

            if (IsJoined)
                throw new AlreadySentException("This user already joined");

            if (Invitations.Last().Status is not InvitationStatus.Pending)
                throw new AlreadySentException("You do not have a pending invitation with this user");

            if (Type is SubscriptionType.Personal)
                throw new BusinessRuleViolationException("this subscription's type is invalid");


            ApplyNewChange(command.ToEvent(Sequence + 1));
        }

        public void AcceptInvitation(AcceptInvitationCommand command)
        {
            if (IsJoined)
                throw new AlreadySentException("This user already joined");

            if (Invitations.Last().Status is not InvitationStatus.Pending)
                throw new AlreadySentException("You do not have a pending invitation with this user");

            if (Type is SubscriptionType.Personal)
                throw new BusinessRuleViolationException("this subscription's type is invalid");

            ApplyNewChange(command.ToEvent(Sequence + 1));
            ApplyNewChange(command.ToJoinedEvent(Sequence + 1));
        }


        public void CancelInvitation(CancelInvitationCommand command)
        {
            if (command.UserId != OwnerId)
                throw new BusinessRuleViolationException("you are not allowed to cancel this invitation");
            if (IsJoined)
            {
                throw new AlreadySentException("This user already joined");
            }
            if (Invitations.Last().Status is not InvitationStatus.Pending)
            {
                throw new AlreadySentException("You do not have a pending invitation with this user");
            }
            if (Type is SubscriptionType.Personal)
            {
                throw new BusinessRuleViolationException("this subscription's type is invalid");
            }

            ApplyNewChange(command.ToEvent(Sequence + 1));

        }

        public void SendInvitation(SendInvitationCommand command)
        {
            if (IsJoined)
            {
                throw new AlreadySentException("This user already joined");
            }
            if (Invitations.Any() &&( Invitations.Last().Status is  InvitationStatus.Accepted || Invitations.Last().Status is InvitationStatus.Pending))
            {
                throw new AlreadySentException("You Already have an invitation with this user");
            }
            if (Type is SubscriptionType.Personal)
            {
                throw new BusinessRuleViolationException("this subscription's type is invalid");
            }

            ApplyNewChange(command.ToEvent(Sequence + 1));

        } 
        internal void ChangePermission(ChangePermissionCommand command)
        {
            //if(command.UserId != UserId)
            //{ 
            //    throw new BusinessRuleViolationException("only owner can change permissions"); 
            //}
            if (!IsJoined)
            {
                throw new AlreadySentException("Cannot change permission of not joined members");
            }

            ApplyNewChange(command.ToEvent(Sequence + 1));
        }

        internal void JoinMember(JoinMemberCommand command)
        { 
            if (IsJoined)
            {
                throw new AlreadySentException("Member already joined");
            }
     
            if (Type is SubscriptionType.Personal)
            {
                throw new BusinessRuleViolationException("this subscription's type is invalid");
            }

            ApplyNewChange(command.ToEvent(Sequence + 1));
        }
        internal void Leave(LeaveSubscriptionCommand command)
        {
            if (command.MemberId != MemberId)
            {
                throw new BusinessRuleViolationException("cannot leave from Subscription that you are not in");
            }
            if (!IsJoined)
            {
                throw new BusinessRuleViolationException("cannot leave Subscription that you are not joined");
            }

            ApplyNewChange(command.ToEvent(Sequence + 1));
        }
        internal void RemoveMember(RemoveMemberCommand command)
        {
            if (!IsJoined)
            {
                throw new BusinessRuleViolationException("cannot leave Subscription that you are not joined");
            }
            ApplyNewChange(command.ToEvent(Sequence + 1));
        }
        protected override void Mutate(Event @event)
        {
            switch (@event)
            {
                case InvitationSent invitationSent:
                    Mutate(invitationSent); break;

                case InvitationCancelled invitationCancelled:
                    Mutate(invitationCancelled); break;

                case InvitationAccepted invitationAccepted:
                    Mutate(invitationAccepted); break;

                case InvitationRejected invitationRejected:
                    Mutate(invitationRejected); break;

                case PermissionChanged permissionChanged:
                        Mutate(permissionChanged); break;

                case MemberJoined memberJoined:
                    Mutate(memberJoined); break;

                case MemberLeft memberLeft:
                    Mutate(memberLeft); break;

                case MemberRemoved memberRemoved:
                    Mutate (memberRemoved); break;

                default:
                    throw new NotImplementedException();
            }
        }

        private void Mutate(InvitationCancelled @event)
        { 
            Sequence = @event.Sequence;
            var invitation = Invitations.MaxBy(x => x.DateTime) ?? throw new ArgumentNullException();
            invitation.Status = InvitationStatus.Cancelled;
        }

        private void Mutate(InvitationRejected @event)
        {
            Sequence = @event.Sequence;
            var invitation = Invitations.MaxBy(x => x.DateTime) ?? throw new ArgumentNullException();
            invitation.Status = InvitationStatus.Rejected;
            
        }

        private void Mutate(InvitationAccepted @event)
        {
            Sequence = @event.Sequence;
            var invitation = Invitations.MaxBy(x => x.DateTime) ?? throw new ArgumentNullException();
            invitation.Status = InvitationStatus.Accepted;
        }

        private void Mutate(InvitationSent @event)
        {
            Sequence = @event.Sequence;
            Id = @event.AggregateId;
            OwnerId = Guid.Parse(@event.UserId); 
            MemberId = @event.Data.MemberId; 
            Invitations.Add(Invitation.Create(@event.Data.InvitationId, @event.Data.UserId, @event.Data.SubscriptionId));
        }

        private void Mutate(PermissionChanged @event)
        {
            Sequence = @event.Sequence;
            Permission = @event.Data.Permission;
        }

        private void Mutate(MemberJoined@event)
        {
            Sequence = @event.Sequence;
            Id = @event.AggregateId;
            Permission = @event.Data.Permission;
            MemberId = @event.Data.MemberId;
            IsJoined = true;
        }
        private void Mutate(MemberLeft @event)
        {
            Sequence = @event.Sequence;
            IsJoined = false;
        }
        private void Mutate(MemberRemoved @event)
        {
            Sequence = @event.Sequence;
            IsJoined = false;
        }


    }
}
