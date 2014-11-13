using AE.News;
using Quartz;
using Quartz.Impl;
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
    public class InsomniaDaemon
    {
#if DEBUG
        /* dev */
        public const double UPDATE_INTERVAL_IN_MINUTES = 0.3;
        public const String API_URI = "http://localhost:65431/api/MakeRequest";
        public const String CALLBACK_URI = "http://localhost:65430/api/Insomnia";
#else
        /* azure */
        public const double UPDATE_INTERVAL_IN_MINUTES = 10;
        public const String API_URI = "http://aeinsomnia.azurewebsites.net/api/MakeRequest";
        public const String CALLBACK_URI = "http://anttieskola.azurewebsites.net/api/Insomnia";
#endif
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
                NewsContext nc = await NewsContext.GetInstance();
                await nc.Maintenance();
            }
            catch (Exception e)
            {
                Debug.WriteLine("InsomniaDeamon - Maintenance - Fatal exception: {0}", e.Message);
            }
            Start();
        }

        /// <summary>
        /// Start
        /// </summary>
        public void Start()
        {
            Debug.WriteLine("InsomniaDaemon - Start");
            String label = DateTime.Now.Ticks.ToString(); // required as this job is created inside other job
            IJobDetail iJob = JobBuilder.Create<InsomniaApiRequest>().WithIdentity("ID_JOB_" + label).Build();
            ITrigger iTrigger = TriggerBuilder.Create()
                .WithIdentity("ID_TRIGGER_" + label)
                .StartAt(DateTimeOffset.Now.AddMinutes(UPDATE_INTERVAL_IN_MINUTES))
                .Build();
            _scheduler.ScheduleJob(iJob, iTrigger);
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
                // making new request
                InsomniaDaemon.Instance.Start();
            }
        }
    }
}
