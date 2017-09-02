using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(www.xinStar.com.Startup))]
namespace www.xinStar.com
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
