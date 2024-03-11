using Microsoft.EntityFrameworkCore;
using SubscriptionQuery.Infrastructure.Presistance.Entities;

namespace SubscriptionQuery.Infrastructure.Presistance
{
    public class ApplicationDatabase : DbContext
    {
         
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<UserSubscription> Subscriptions { get; set; }
        
        public ApplicationDatabase()
        {
            
        }

        public ApplicationDatabase(DbContextOptions<ApplicationDatabase> options): base (options)
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invitation>().Property(x => x.Status).HasConversion<string>();
            modelBuilder.Entity<UserSubscription>().Property(x => x.Permissions).HasConversion<string>();
            base.OnModelCreating(modelBuilder);
        }
         

    }
    
}
