using SubscriptionQuery.Commands.InvitationSent;
using SubscriptionQuery.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubscriptionQuery.Infrastructure.Presistance.Entities;

public class Invitation
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
    public Guid UserSubscriptionId { get; set; }
    public UserSubscription UserSubscription { get; set; }
    public InvitationStatus Status { get; set; }
    public Permissions Permission{ get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }

    public static Invitation Create(InvitationSent request)
    {
        return new Invitation
        {
            Id = request.Data.InvitationId,
            UserSubscriptionId = request.AggregateId,
            DateCreated = request.DateTime,
            Status = InvitationStatus.Pending,
            Permission = request.Data.Permission,
        };
    }
}