using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Doubble.Startup))]
namespace Doubble
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
