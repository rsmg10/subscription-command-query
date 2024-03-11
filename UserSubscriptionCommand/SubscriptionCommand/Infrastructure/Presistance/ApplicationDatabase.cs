using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SubscriptionCommand.Domain;
using SubscriptionCommand.Events;

namespace SubscriptionCommand.Infrastructure.Presistance
{
    public class ApplicationDatabase : DbContext
    {
        public ApplicationDatabase()
        {
            
        } 

        public ApplicationDatabase(DbContextOptions<ApplicationDatabase> options): base(options)
        {

        }

        public DbSet<EventEntity> Events { get; set; }
        public DbSet<Outbox> Outbox { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            // optionsBuilder.UseSqlServer(
            //     "Data Source=.;Database=AAASubscriptionCommand;Trusted_Connection=true;TrustServerCertificate=True");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDatabase).Assembly);

            base.OnModelCreating(modelBuilder);
        }

    }
}
