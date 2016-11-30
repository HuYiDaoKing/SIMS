using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SIMS.Web.Startup))]
namespace SIMS.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
