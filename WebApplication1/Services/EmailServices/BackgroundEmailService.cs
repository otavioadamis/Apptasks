using WebApplication1.Models.Email;
using NCrontab;
using WebApplication1.Interfaces;

namespace WebApplication1.Services.EmailServices
{
    public class BackgroundEmailService : IHostedService
    {
        private readonly IEmailService _emailService;
        private readonly IProjectService _projectService;
        private CrontabSchedule _schedule;
        private DateTime _nextRun;

        public BackgroundEmailService(IEmailService emailService, IProjectService projectService)
        {
            _emailService = emailService;
            _projectService = projectService;
            // Define your desired CRON expression for once a day at a specific time (2:00 AM)
            _schedule = CrontabSchedule.Parse("36 23 * * *");
            _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;

                if (now > _nextRun)
                {
                    await _emailService.SendEmailAsync(_projectService.GetEmails());
                    _nextRun = _schedule.GetNextOccurrence(now);
                }

                // Calculate the time until the next scheduled run
                var timeUntilNextRun = _nextRun - now;

                // Ensure the delay interval is at least 2 minutes to avoid busy-waiting
                var delayInterval = timeUntilNextRun > TimeSpan.FromMinutes(2) ? timeUntilNextRun : TimeSpan.FromMinutes(2);

                await Task.Delay(delayInterval, cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {

            return Task.CompletedTask;

        }
    }
}
