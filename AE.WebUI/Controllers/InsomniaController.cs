using AE.Insomnia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace AE.WebUI.Controllers
{
    public class InsomniaController : ApiController
    {
        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            // todo: flood control
            await InsomniaDaemon.Instance.Maintenance();
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
