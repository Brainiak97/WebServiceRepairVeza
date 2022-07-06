using Quartz;
using Quartz.Impl;

namespace WebService.Quartz
{
    public static class ArchivingScheduler
    {
        public static async Task Start(IServiceProvider serviceProvider)
        {
            if (serviceProvider != null)
            {
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                scheduler.JobFactory = serviceProvider.GetService<ArchivingFactory>()!;
                await scheduler.Start();

                IJobDetail jobDetail = JobBuilder.Create<ArchivingJob>().Build();
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("ArchivingTrigger", "default")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromDays(7))
                    .RepeatForever())
                    .Build();

                await scheduler.ScheduleJob(jobDetail, trigger);
            }
        }
    }
}
