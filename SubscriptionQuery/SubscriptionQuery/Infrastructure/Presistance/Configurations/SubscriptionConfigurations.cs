using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionQuery.Infrastructure.Presistance.Entities;

namespace SubscriptionQuery.Infrastructure.Presistance.Configurations;

public class SubscriptionConfigurations : IEntityTypeConfiguration<UserSubscription> 
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder
            .HasMany<Invitation>(x=> x.Invitations)
            .WithOne(x => x.UserSubscription)
            .HasForeignKey(x => x.UserSubscriptionId);
    }
}