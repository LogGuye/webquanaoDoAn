using System;
using System.Net.NetworkInformation;
using Owin;
using Microsoft.Owin;
using Quanlycuahangquanao.App_Start;    // <== import namespace AuthConfig của bạn

[assembly: OwinStartup(typeof(Quanlycuahangquanao.Startup))]
namespace Quanlycuahangquanao
{
    public class Startup
    {
        // OWIN sẽ gọi vào đây
        public void Configuration(IAppBuilder app)
        {
            // gọi tiếp phần config authentication
            AuthConfig.ConfigureAuth(app);
        }
    }
}
