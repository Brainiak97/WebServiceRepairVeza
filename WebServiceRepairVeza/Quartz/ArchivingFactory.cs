using Quartz;
using Quartz.Spi;

namespace WebService.Quartz
{
    public class ArchivingFactory : IJobFactory
    {
        protected readonly IServiceScopeFactory serviceScopeFactory;

        public ArchivingFactory(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var job = scope.ServiceProvider.GetService(bundle.JobDetail.JobType) as IJob;
            return job!;
        }

        public void ReturnJob(IJob job)
        {
            //Do something if need
        }
    }
}
