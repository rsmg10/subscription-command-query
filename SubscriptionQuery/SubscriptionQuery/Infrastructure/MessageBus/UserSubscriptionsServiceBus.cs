//using Azure.Messaging.ServiceBus;

//namespace SubscriptionQuery.Infrastructure.MessageBus
//{
//    public class UserSubscriptionsServiceBus
//    {
//        private readonly IConfiguration configuration;
//        public static string  connectionString { get; set; }
//        public UserSubscriptionsServiceBus(IConfiguration configuration)
//        {
//            connectionString = configuration["ServiceBusClient"];
//        }
//        public ServiceBusClient Client { get; } = new(connectionString);
//    }
//}
