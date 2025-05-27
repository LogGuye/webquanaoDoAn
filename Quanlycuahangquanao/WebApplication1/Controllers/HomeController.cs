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
            var products = db.SanPhams.Where(sp => sp.TrangThai == "Mở bán");

            // Nếu không có categoryId hoặc categoryId = 0 => hiển thị Sản phẩm mới
            if (categoryId == null || categoryId == 0)
            {
                products = products.Where(sp => sp.TrangThai == "Mở bán");
            }
            else
            {
                // Theo danh mục
                products = products.Where(sp => sp.DanhMucs.Any(dm => dm.MaDanhMuc == categoryId));

                // Lọc giá
                if (minPrice != null)
                    products = products.Where(sp => sp.GiaBan >= minPrice);
                if (maxPrice != null)
                    products = products.Where(sp => sp.GiaBan <= maxPrice);
            }

            // Lấy danh mục để hiển thị filter sidebar
            ViewBag.CategoryId = categoryId ?? 0;
            ViewBag.Categories = db.DanhMucs.ToList();

            return View(products.ToList());
        }
    }
}
