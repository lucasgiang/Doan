using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Doan.Startup))]
namespace Doan
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
