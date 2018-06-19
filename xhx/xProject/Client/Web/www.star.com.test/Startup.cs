using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(www.star.com.test.Startup))]
namespace www.star.com.test
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
