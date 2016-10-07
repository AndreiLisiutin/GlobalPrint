using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace GlobalPrint.ClientWeb.Helpers.ScheduledActivityChecker
{
    public class ActivityCheckerJobScheduler
    {
        private readonly TimeSpan _callInterval;

        public ActivityCheckerJobScheduler()
        {
            _callInterval = TimeSpan.FromMinutes(double.Parse(WebConfigurationManager.AppSettings["ActivityCheckerCallInterval"]));
        }

        public void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<ActivityCheckerJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(_callInterval)
                    .RepeatForever())
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}