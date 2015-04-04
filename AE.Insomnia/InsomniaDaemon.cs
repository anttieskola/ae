using AE.Funny.Service;
using AE.News;
using AE.News.Service;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AE.Insomnia
{
    /// <summary>
    /// daemon to start and keep one maintenance job running all the time
    /// thats why you find lot of defensive programming here.
    /// </summary>
    public class InsomniaDaemon
    {
#if DEBUG
        /* dev */
        public const double UPDATE_INTERVAL_IN_MINUTES = 1;
        public const String API_URI = "http://localhost:65431/api/MakeRequest";
        public const String CALLBACK_URI = "http://localhost:65430/api/Insomnia";
        public const int NEWS_UPDATE_INTERVAL_IN_MINUTES = 1;
        public const int FUNNY_UPDATE_INTERVAL_IN_MINUTES = 1;
#else
        /* azure */
        public const double UPDATE_INTERVAL_IN_MINUTES = 10;
        public const String API_URI = "http://aeinsomnia.azurewebsites.net/api/MakeRequest";
        public const String CALLBACK_URI = "http://anttieskola.azurewebsites.net/api/Insomnia";
        public const int NEWS_UPDATE_INTERVAL_IN_MINUTES = 15;
        public const int FUNNY_UPDATE_INTERVAL_IN_MINUTES = 45;
#endif
        private const String JOB_PREFIX = "ID_JOB_";
        private const String TRIGGER_PREFIX = "ID_TRIGGER_";
        private IScheduler _scheduler;
        #region lazy singleton
        private static readonly Lazy<InsomniaDaemon> lazy =
            new Lazy<InsomniaDaemon>(() => new InsomniaDaemon());
        public static InsomniaDaemon Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private InsomniaDaemon()
        {
            if (_scheduler == null)
            {
                _scheduler = StdSchedulerFactory.GetDefaultScheduler();
            }

            if (!_scheduler.IsStarted)
            {
                try
                {
                    _scheduler.Start();
                }
                catch (SchedulerException)
                {

                }
            }

        }
        #endregion


        /// <summary>
        /// Maintenance loop
        /// </summary>
        public async Task Maintenance()
        {
            Debug.WriteLine("InsomniaDaemon - Maintenance");
            // wrap maintenance so it won't prevent us from going on
            try
            {
                // We will run news service around every 15 mins
                NewsService ns = NewsService.Instance;
                if (DateTime.UtcNow.Subtract(ns.LastSuccess()).TotalMinutes > NEWS_UPDATE_INTERVAL_IN_MINUTES)
                {
                    await ns.RunAsync();
                }
                // We will run funny service around once per hour
                FunnyService fs = FunnyService.Instance;
                if (DateTime.UtcNow.Subtract(fs.LastSuccess()).TotalMinutes > FUNNY_UPDATE_INTERVAL_IN_MINUTES)
                {
                    await fs.RunAsync();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("InsomniaDeamon - Maintenance - Fatal exception: {0}", e.Message);
            }
            if (!isJobScheduled())
            {
                scheduleJob();
            }
        }

        /// <summary>
        /// Start
        /// </summary>
        public void Start()
        {
            Debug.WriteLine("InsomniaDaemon - Start");
            Task.Factory.StartNew(() => Maintenance().Wait());
        }

        /// <summary>
        /// Stop
        /// Current not used as can't call this from httpapplication dispose
        /// as it is called few seconds after startup.
        /// </summary>
        public void Stop()
        {
            Debug.WriteLine("InsomniaDaemon - Stop");
            if (_scheduler != null)
            {
                if (_scheduler.IsStarted && !_scheduler.IsShutdown)
                {
                    _scheduler.Shutdown(true);
                    _scheduler = null;
                }
            }
        }

        /// <summary>
        /// schedule job
        /// </summary>
        private void scheduleJob()
        {
            Debug.WriteLine("InsomniaDaemon - scheduleJob");
            String label = DateTime.Now.Ticks.ToString(); // required as this job is created inside other job
            IJobDetail iJob = JobBuilder.Create<InsomniaApiRequest>().WithIdentity(JOB_PREFIX + label).Build();
            ITrigger iTrigger = TriggerBuilder.Create()
                .WithIdentity(TRIGGER_PREFIX + label)
                .StartAt(DateTimeOffset.Now.AddMinutes(UPDATE_INTERVAL_IN_MINUTES))
                .Build();
            _scheduler.ScheduleJob(iJob, iTrigger);
        }

        /// <summary>
        /// check is job already scheduled
        /// </summary>
        /// <returns></returns>
        private bool isJobScheduled()
        {
            Debug.WriteLine("InsomniaDaemon - isJobScheduled");
            int count = 0;
            foreach (String group in _scheduler.GetJobGroupNames())
            {
                foreach (JobKey jk in _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(group)))
                {
                    String jobName = jk.Name;
                    Debug.WriteLine("InsomniaDaemon - isJobScheduled - job:{0}", jobName);
                    if (jobName.Contains(JOB_PREFIX))
                    {
                        count++;
                    }
                }
            }
            Debug.WriteLine("InsomniaDaemon - isJobScheduled - count:{0}", count);
            if (count > 0)
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Job to request call back using insomnia web service.
    /// </summary>
    internal class InsomniaApiRequest : IJob
    {
        public async void Execute(IJobExecutionContext context)
        {
            Debug.WriteLine("InsomniaApiRequest - Execute");
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(InsomniaDaemon.API_URI);
                req.ContentType = "application/json; charset=utf-8";
                req.Method = "POST";
                using (var sw = new StreamWriter(req.GetRequestStream()))
                {
                    sw.Write("{\"url\":\"" + InsomniaDaemon.CALLBACK_URI + "\"}");
                    sw.Flush();
                    sw.Close();
                }
                var res = (HttpWebResponse)await req.GetResponseAsync();
                if (res.StatusCode != HttpStatusCode.OK)
                {
                    Debug.WriteLine("InsomniaApiRequest - Execute - ResponseError, StatusCode: {0}", res.StatusCode);
                }
            }
            catch (UriFormatException)
            {
                // invalid api uri, can't recover from this.
                Debug.WriteLine("InsomniaApiRequest - Execute - Fatal error, invalid api url");
            }
            catch (WebException)
            {
                // can't access server
                Debug.WriteLine("InsomniaApiRequest - Execute - Can't access api server");
                // launch maintenance from here
                Debug.WriteLine("InsomniaApiRequest - Execute - launching maintenance");
                await InsomniaDaemon.Instance.Maintenance();
                // making new request
                InsomniaDaemon.Instance.Start();
            }
        }
    }
}
