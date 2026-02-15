namespace GymManagementSystem.Application.Exceptions
{
    public class SubscriptionExpiredException : Exception
    {
        public SubscriptionExpiredException(DateTime endDate)
            : base($"Subscription expired on {endDate:yyyy-MM-dd}. Please renew.")
        {
        }
    }
}