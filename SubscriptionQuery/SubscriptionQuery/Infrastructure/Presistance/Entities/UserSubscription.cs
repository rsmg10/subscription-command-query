 
using SubscriptionQuery.Commands.InvitationAccepted;
using SubscriptionQuery.Commands.InvitationCancelled;
using SubscriptionQuery.Commands.InvitationRejected;
using SubscriptionQuery.Commands.InvitationSent;
using SubscriptionQuery.Commands.MemberJoined;
using SubscriptionQuery.Commands.MemberLeft;
using SubscriptionQuery.Commands.MemberRemoved;
using SubscriptionQuery.Commands.PermissionChanged;
using SubscriptionQuery.Domain.Enums;

namespace SubscriptionQuery.Infrastructure.Presistance.Entities;

public partial class UserSubscription : BaseEntity
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public Guid SubscriptionId { get; set; }
    public Guid AccountId { get; set; }
    public Guid MemberId { get; set; }
    public bool IsJoined { get; set; }
    public Permissions Permissions { get; set; }
    public SubscriptionType SubscriptionType { get; set; }
    public ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();

    public UserSubscription()
    {
    }
    internal static UserSubscription Create(InvitationSent request)
    {
        return new UserSubscription
        {
            Id = request.AggregateId,
            AccountId = request.Data.AccountId,
            MemberId = request.Data.MemberId,
            OwnerId = request.Data.UserId,
            SubscriptionId = request.Data.SubscriptionId, 
            DateCreated = request.DateTime,
        };
    }

    internal void Apply(MemberRemoved request)
    {
        IsJoined = false;
        Sequence = request.Sequence;
        DateUpdated = request.DateTime;
    }

    internal void Apply(InvitationAccepted request)
    {
        var invitation = Invitations.MaxBy(x => x.DateCreated)!;
        invitation.Status = InvitationStatus.Accepted;
        invitation.DateUpdated = request.DateTime;
        Permissions = invitation.Permission;
        Sequence = request.Sequence;  
    }

    internal void Apply(InvitationCancelled request)
    {
        var invitation = Invitations.MaxBy(x => x.DateCreated)!;
        invitation.Status = InvitationStatus.Cancelled;
        invitation.DateUpdated = request.DateTime;
        Sequence = request.Sequence;
    }

    internal void Apply(InvitationRejected request)
    {
        var invitation = Invitations.MaxBy(x => x.DateCreated)!;
        invitation.Status = InvitationStatus.Rejected;
        invitation.DateUpdated = request.DateTime;
        Sequence = request.Sequence; 
    }

    internal void Apply(InvitationSent request)
    {
        Invitations.Add(Invitation.Create(request));

        Sequence = request.Sequence;
        DateCreated = DateCreated == DateTime.MinValue ? request.DateTime : DateCreated;
        DateUpdated = request.DateTime;
    }

    internal void Apply(MemberJoined request)
    {
        Id = request.AggregateId;
        Sequence = request.Sequence;
        DateUpdated = request.DateTime;
        Permissions = request.Data.Permission;
        AccountId = request.Data.AccountId;
        OwnerId = request.Data.UserId;
        SubscriptionId = request.Data.SubscriptionId;
        MemberId = request.Data.MemberId;
        IsJoined = true;

        DateCreated = DateCreated == DateTime.MinValue ? request.DateTime : DateCreated;
    }

    internal void Apply(MemberLeft request)
    {
        IsJoined = false; 
        Sequence = request.Sequence;
        DateUpdated = request.DateTime;
    }

    internal void Apply(PermissionChanged request)
    {
        Sequence = request.Sequence;
        DateUpdated = request.DateTime;
        Permissions = request.Data.Permission;
    }
}
