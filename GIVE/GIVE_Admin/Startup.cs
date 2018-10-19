using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GIVE_Admin.Startup))]
namespace GIVE_Admin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
