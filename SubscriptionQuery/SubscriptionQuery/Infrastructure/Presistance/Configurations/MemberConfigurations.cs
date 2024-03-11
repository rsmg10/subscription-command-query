using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionQuery.Infrastructure.Presistance.Entities;

namespace SubscriptionQuery.Infrastructure.Presistance.Configurations;

public class MemberConfigurations : IEntityTypeConfiguration<Member> 
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
    }
}