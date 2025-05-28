using System.Linq;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ListController : Controller
    {
        private QLCHQA db = new QLCHQA();

        // Trang chi tiết danh mục (danh sách sản phẩm theo danh mục)
        public ActionResult ChiTiet(int id)
        {
            var danhMuc = db.DanhMucs.Find(id);
            if (danhMuc == null) return HttpNotFound();

            // Lấy tất cả danh mục để show menu + biết mục nào active
            var allCategories = db.DanhMucs.ToList();

            // Lấy sản phẩm theo danh mục
            var spTheoDM = db.SanPhams.Where(x => x.DanhMucs.Select(y => y.MaDanhMuc).Contains(id)).ToList();

            ViewBag.DanhMuc = danhMuc;
            ViewBag.AllCategories = allCategories;
            ViewBag.SelectedCategoryId = id;
            return View(spTheoDM);
        }
    }
}
    