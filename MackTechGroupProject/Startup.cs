//Testing Commit to github - Andy

using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MackTechGroupProject.Startup))]
namespace MackTechGroupProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
