using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionCommand.Domain;

namespace SubscriptionCommand.Infrastructure.Presistance.Configurations
{
    public class OutboxConfigurations : IEntityTypeConfiguration<Outbox>
    {
        public void Configure(EntityTypeBuilder<Outbox> builder)
        {
            builder.HasOne(x => x.Event)
                .WithOne()
                .HasForeignKey<Outbox>(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
