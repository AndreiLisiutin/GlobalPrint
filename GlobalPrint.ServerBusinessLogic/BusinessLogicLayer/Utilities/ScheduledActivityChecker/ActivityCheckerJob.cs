using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Users;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Utilities.ScheduledActivityChecker
{
    public class ActivityCheckerJob : IJob
    {
        private const int _thresholdInMinutes = 30;
        private readonly TimeSpan _threshold = TimeSpan.FromMinutes(_thresholdInMinutes);
        private readonly TimeSpan _callInterval = TimeSpan.FromMinutes(5);

        private IUserUnit _userUnit;

        public ActivityCheckerJob(IUserUnit userUnit)
        {
            _userUnit = userUnit;
        }

        public void Execute(IJobExecutionContext context)
        {
            var inactiveUserList = _userUnit.GetInactiveUsers(_threshold, _callInterval);
        }
    }

    public class ActivityCheckerJobScheduler
    {
        private const int _thresholdInMinutes = 30;

        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<ActivityCheckerJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(_thresholdInMinutes)
                    .RepeatForever())
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}
