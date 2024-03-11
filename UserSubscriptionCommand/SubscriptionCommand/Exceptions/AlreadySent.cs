namespace SubscriptionCommand.Exceptions
{
    public class AlreadySentException : Exception
    {
        public AlreadySentException(string message) : base(message)
        {
            
        }
    }
}
