using Owin;

namespace Quanlycuahangquanao.App_Start
{
    public static class AuthConfig
    {
        // Phải có thân (body) dù là rỗng
        public static void ConfigureAuth(IAppBuilder app)
        {
            // Ví dụ:
            // app.UseCookieAuthentication(new CookieAuthenticationOptions { ... });
            // app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            //
            // Nếu bạn chưa dùng authentication, để trống cũng không lỗi:
        }
    }
}
