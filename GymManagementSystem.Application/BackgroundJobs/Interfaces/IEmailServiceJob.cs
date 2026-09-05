namespace GymManagementSystem.Application.BackgroundJobs.Interfaces
{
    public interface IEmailServiceJob
    {
        Task SendWelcomeEmailAsync(string email, string name);
    }
}
