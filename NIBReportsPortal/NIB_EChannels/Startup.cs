using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NIB_EChannels.Startup))]
namespace NIB_EChannels
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
