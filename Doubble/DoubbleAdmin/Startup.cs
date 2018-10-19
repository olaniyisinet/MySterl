using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DoubbleAdmin.Startup))]
namespace DoubbleAdmin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
