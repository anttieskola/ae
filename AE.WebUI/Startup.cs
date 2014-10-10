using System;
using System.Threading.Tasks;
using Owin;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(AE.WebUI.Startup))]
namespace AE.WebUI
{
    /// <summary>
    /// owin startup, ran after app startup by Microsoft.Owin.Host.SystemWeb
    /// </summary>
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Configure(app);
        }
    }
}
