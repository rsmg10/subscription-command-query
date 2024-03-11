using System.Runtime.InteropServices.JavaScript;
using SubscriptionCommand.Domain.Enums;

namespace SubscriptionCommand.Domain
{
    public class Invitation
    {
        private Invitation(Guid InvitationId, Guid userId, Guid subscriptionId) {
            Id = InvitationId;
            UserId = userId;
            SubscriptionId = subscriptionId;
            Status = InvitationStatus.Pending;
            DateTime = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public Guid UserId{ get; set; }
        public Guid SubscriptionId{ get; set; }
        public InvitationStatus Status { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;


        public static Invitation Create(Guid InvitationId, Guid userId, Guid subscriptionId)
        {
            return new(InvitationId, userId, subscriptionId);
        }
    }
}
