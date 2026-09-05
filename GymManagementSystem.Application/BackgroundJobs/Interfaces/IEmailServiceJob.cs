namespace GymManagementSystem.Application.BackgroundJobs.Interfaces
{
    public interface IEmailServiceJob
    {
        Task SendWelcomeEmailAsync(int memberId);
    }
}
