using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Evoting.Startup))]
namespace Evoting
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
