using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionCommand.Domain;
using SubscriptionCommand.Events;

namespace SubscriptionCommand.Infrastructure.Presistance.Configurations
{
    public class EventConfigurations : IEntityTypeConfiguration<EventEntity>
    {
        public void Configure(EntityTypeBuilder<EventEntity> builder)
        {
            builder.HasIndex(x => new { x.Sequence, x.AggregateId }).IsUnique();
        }
        // todo: demo command side generic

    }
}
