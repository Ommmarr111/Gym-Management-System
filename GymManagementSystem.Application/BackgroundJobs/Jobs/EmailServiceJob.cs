using GymManagementSystem.Application.BackgroundJobs.Interfaces;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.Application.BackgroundJobs.Jobs
{
    public class EmailServiceJob : IEmailServiceJob
    {
        private readonly ILogger<EmailServiceJob> _logger;

        public EmailServiceJob(ILogger<EmailServiceJob> logger)
        {
            _logger = logger;
        }
        public async Task SendWelcomeEmailAsync(string email, string name)
        {
            // Sending email logic here using an email service provider (e.g., SendGrid, SMTP, etc.)
            _logger.LogInformation($"Sending welcome email to {name} at {email}...");

            // Simulate email sending delay

            await Task.Delay(2000);
            _logger.LogInformation("Email sent successfully!");
        }
    }
}
