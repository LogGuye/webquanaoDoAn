using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly QLCHQA db = new QLCHQA();

        public ActionResult Index(int? categoryId, string filter = null, double? minPrice = null, double? maxPrice = null)
        {
            var products = db.SanPhams.Where(sp => sp.TrangThai == "True" && sp.LaSanPhamMoi == true);

            // Lấy tất cả danh mục để show menu + biết mục nào active
            var allCategories = db.DanhMucs.ToList();

            // Sản phẩm mới (khi chưa chọn danh mục)
            if (categoryId == null || categoryId == 0)
            {
                products = products.Where(sp => sp.LaSanPhamMoi == true);
            }
            else
            {
                products = products.Where(sp => sp.DanhMucs.Any(dm => dm.MaDanhMuc == categoryId));
                if (minPrice != null) products = products.Where(sp => sp.GiaBan >= minPrice);
                if (maxPrice != null) products = products.Where(sp => sp.GiaBan <= maxPrice);
            }

            ViewBag.CategoryId = categoryId ?? 0;
            ViewBag.Categories = db.DanhMucs.ToList();
            ViewBag.AllCategories = allCategories;  
            return View(products.ToList());
        }
    }
}
