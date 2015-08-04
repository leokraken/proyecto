using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SAREM.Web.Startup))]
namespace SAREM.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
