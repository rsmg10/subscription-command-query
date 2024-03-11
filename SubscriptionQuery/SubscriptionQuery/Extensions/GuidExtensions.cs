
namespace SubscriptionQuery.Extensions
{
    public static class GuidExtensions
    {

        public static Guid ToGuid(this string guidString)
        {
            return Guid.TryParse(guidString, out _) ? Guid.Parse(guidString) : throw new Exception("could not parse guid");
        }
    }
 
}
