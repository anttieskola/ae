using AE.Insomnia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace AE.WebUI.Controllers.Api
{
    public class InsomniaController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage MakeRequest()
        {
            // fire and forget
            Task.Factory.StartNew(() => InsomniaDaemon.Instance.Maintenance().Wait());
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
