using GymManagementSystem.Application.BackgroundJobs.Interfaces;
using GymManagementSystem.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.Application.BackgroundJobs.Jobs
{
    public class EmailServiceJob : IEmailServiceJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EmailServiceJob> _logger;

        public EmailServiceJob(IUnitOfWork unitOfWork, ILogger<EmailServiceJob> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task SendWelcomeEmailAsync(int memberId)
        {
            var member = await _unitOfWork.Members.GetByIdAsync(memberId);

            if (member is null)
            {
                _logger.LogWarning("Member {MemberId} no longer exists, skipping welcome email.", memberId);
                return;
            }

            if (member.WelcomeEmailSentAt is not null)
            {
                _logger.LogInformation("Welcome email already sent to member {MemberId} at {SentAt}, skipping.",
                    memberId, member.WelcomeEmailSentAt);
                return;
            }

            _logger.LogInformation("Sending welcome email to {Name} at {Email}...", member.FirstName, member.Email);

            // Actual send logic here (SendGrid/SMTP) — simulated for now
            await Task.Delay(2000);

            member.WelcomeEmailSentAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Email sent successfully to member {MemberId}.", memberId);
        }
    }
}