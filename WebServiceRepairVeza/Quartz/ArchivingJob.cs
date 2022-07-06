using BLL.Services;
using Quartz;

namespace WebService.Quartz
{
    [DisallowConcurrentExecution]
    public class ArchivingJob : IJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ArchivingJob> _logger;

        public ArchivingJob(IServiceScopeFactory serviceScopeFactory, ILogger<ArchivingJob> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var service = scope.ServiceProvider.GetService<RepairLogService>();

                    if (service != null)
                        await service.ToArchiveAllCompleted();
                }
                _logger.LogInformation($"The {nameof(context)} execute archiving was successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(context)} execute archiving failed.", ex);
            }
        }
    }
}
