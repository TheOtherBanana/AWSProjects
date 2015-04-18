using PricePeriodicTask;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmazonPriceUpdateWebAPI.ScheduledJobUtils
{
    public class PeriodicJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            PeriodicTask task = new PeriodicTask();
            task.CheckPriceAndSendMail(APIConfigManager.AdminAccountId);
        }
    }

    public class JobScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<PeriodicJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                                .WithIdentity("ProductAPIPeriodicTask", "StandardGroup")
                                .StartNow()
                                .WithSimpleSchedule(x => x
                                    .WithIntervalInHours(6)
                                    .RepeatForever())
                                .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}