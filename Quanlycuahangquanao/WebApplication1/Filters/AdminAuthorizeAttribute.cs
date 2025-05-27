using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Filters
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var maLoaiTK = httpContext.Session["MaLoaiTK"];
            // Chỉ cho phép nếu role là 1 (admin)
            return (maLoaiTK != null && maLoaiTK.ToString() == "1");
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Nếu không phải admin, chuyển về trang đăng nhập (hoặc trang nào bạn muốn)
            filterContext.Result = new RedirectResult("~/Account/Login");
        }
    }
}