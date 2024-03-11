using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionQuery.Infrastructure.Presistance.Entities;

namespace SubscriptionQuery.Infrastructure.Presistance.Configurations;

public class InvitationConfigurations : IEntityTypeConfiguration<Invitation> 
{
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
    }
}