using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AE.News
{
    public class NewsDaemon
    {
        // quartz scheduler
        private IScheduler _scheduler;

        #region lazy singleton
        private static readonly Lazy<NewsDaemon> lazy =
            new Lazy<NewsDaemon>(() => new NewsDaemon());
        public static NewsDaemon Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private NewsDaemon()
        {
            // noop
        }
        #endregion

        /// <summary>
        /// start news update
        /// </summary>
        /// <param name="updateIntervalInMinutes"></param>
        /// <param name="cleanupIntervalInHours"></param>
        /// <returns>true if started otherwise false</returns>
        public bool Start(int updateIntervalInMinutes = 5, int cleanupIntervalInHours = 1)
        {
            if (updateIntervalInMinutes < 3)
            {
                throw new ArgumentException("intervalInMinutes must be >= 3");
            }

            if (_scheduler == null)
            {
                _scheduler = StdSchedulerFactory.GetDefaultScheduler();
            }

            if (!_scheduler.IsStarted)
            {
                try
                {
                    _scheduler.Start();
                    // update job
                    IJobDetail updateJob = JobBuilder.Create<NewsUpdateJob>().WithIdentity("newsupdatejob").Build();
                    ITrigger updateTrigger = TriggerBuilder.Create()
                        .WithIdentity("newsupdatetrigger")
                        .StartAt(DateTimeOffset.Now.AddMinutes(updateIntervalInMinutes))
                        .WithSimpleSchedule(s => s.WithIntervalInMinutes(updateIntervalInMinutes).RepeatForever())
                        .Build();
                    _scheduler.ScheduleJob(updateJob, updateTrigger);

                    // cleanup job
                    IJobDetail cleanupJob = JobBuilder.Create<NewsCleanupJob>().WithIdentity("newscleanupjob").Build();
                    ITrigger cleanupTrigger = TriggerBuilder.Create()
                        .WithIdentity("newscleanuptrigger")
                        .StartAt(DateTimeOffset.Now.AddHours(cleanupIntervalInHours))
                        .WithSimpleSchedule(s => s.WithIntervalInHours(cleanupIntervalInHours).RepeatForever())
                        .Build();
                    _scheduler.ScheduleJob(cleanupJob, cleanupTrigger);

                    Debug.WriteLine("NewsDaemon - Started");
                    return true;
                }
                catch (SchedulerException e)
                {
                    Debug.WriteLine(e);
                }
            }
            return false;
        }

        /// <summary>
        /// stop news update
        /// </summary>
        /// <returns>true if stopped otherwise false</returns>
        public bool Stop()
        {
            if (_scheduler != null)
            {
                if (_scheduler.IsStarted && !_scheduler.IsShutdown)
                {
                    _scheduler.Shutdown(true);
                    _scheduler = null;
                    Debug.WriteLine("NewsDaemon - Stopped");
                    return true;
                }
            }
            return false;
        }
    }

    internal class NewsUpdateJob : IJob
    {
        public async void Execute(IJobExecutionContext context)
        {
            Debug.WriteLine("NewsDaemon - NewsUpdateJob - Execute");
            NewsContext c = await NewsContext.GetInstance();
            await c.Update();
        }
    }

    internal class NewsCleanupJob : IJob
    {

        public async void Execute(IJobExecutionContext context)
        {
            Debug.WriteLine("NewsDaemon - NewsCleanupJob - Execute");
            NewsContext c = await NewsContext.GetInstance();
            c.Cleanup();
        }
    }
}
