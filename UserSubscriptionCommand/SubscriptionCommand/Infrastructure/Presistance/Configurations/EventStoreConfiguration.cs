    
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using Newtonsoft.Json;
//using SubscriptionCommand.Events;
//using System.Text.Json;

//namespace Core.Application.Persistence.Configurations;

//public class EventStoreConfiguration<T, TData> : IEntityTypeConfiguration<T> where T : Event<TData> 
//{
//    public void Configure(EntityTypeBuilder<T> builder)
//    {
//        builder.Property(e => e.Data).HasConversion(
//            v => JsonConvert.SerializeObject(v),
//            v => JsonConvert.DeserializeObject<TData>(v)!
//        ).HasColumnName("Data");
//    }
//}