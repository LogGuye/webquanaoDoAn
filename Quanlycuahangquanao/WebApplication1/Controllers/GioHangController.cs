using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class GioHangController : Controller
    {
        private readonly QLCHQA db = new QLCHQA();
        // GET: GioHang
        public ActionResult Index()
        {
            // Lấy mã khách hàng hiện tại từ Session
            int maKH = Convert.ToInt32(Session["MaKH"]);
            using (var db = new QLCHQA())
            {
                var cart = db.GioHangs
                    .Where(g => g.MaKH == maKH)
                    .Include("SanPham")
                    .ToList();
                return View(cart);
            }
        }

        [HttpPost]
        public ActionResult UpdateCart(List<GioHang> items)
        {
            int maKH = Convert.ToInt32(Session["MaKH"]);
            using (var db = new QLCHQA())
            {
                foreach (var update in items)
                {
                    var gh = db.GioHangs.FirstOrDefault(g => g.MaKH == maKH && g.MaSP == update.MaSP);
                    if (gh != null)
                        gh.SoLuong = update.SoLuong;
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Remove(int id)
        {
            int maKH = Convert.ToInt32(Session["MaKH"]);
            using (var db = new QLCHQA())
            {
                var item = db.GioHangs.FirstOrDefault(g => g.MaKH == maKH && g.MaSP == id);
                if (item != null)
                {
                    db.GioHangs.Remove(item);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult AddToCart(int id)
        {
            // Kiểm tra đăng nhập
            if (Session["MaKH"] == null)
            {
                TempData["Message"] = "Vui lòng đăng nhập để thêm vào giỏ hàng.";
                return RedirectToAction("Login", "Account");
            }

            int maKH = Convert.ToInt32(Session["MaKH"]);
            var sp = db.SanPhams.Find(id);
            if (sp == null)
            {
                TempData["Error"] = "Sản phẩm không tồn tại!";
                return RedirectToAction("Index", "Home");
            }

            // Tìm giỏ hàng hiện tại
            var gio = db.GioHangs.FirstOrDefault(g => g.MaKH == maKH && g.MaSP == id);
            if (gio != null)
            {
                gio.SoLuong = (gio.SoLuong ?? 0) + 1;
            }
            else
            {
                gio = new GioHang
                {
                    MaKH = maKH,
                    MaSP = id,
                    SoLuong = 1
                };
                db.GioHangs.Add(gio);
            }
            db.SaveChanges();

            TempData["Message"] = "Đã thêm vào giỏ hàng!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DatHang(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Checkout", model);

            int maKH = Convert.ToInt32(Session["MaKH"]);
            var cart = db.GioHangs.Include("SanPham").Where(x => x.MaKH == maKH).ToList();
            if (cart == null || cart.Count == 0)
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }

            // Tạo giao hàng
            var gh = new GiaoHang
            {
                TenNguoiNhan = model.TenNguoiNhan,
                DiaChiNhan = model.DiaChiNhan,
                SoDienThoai = model.SoDienThoai,
                MaKH = maKH,
            };
            db.GiaoHangs.Add(gh);
            db.SaveChanges();

            // Tạo đơn hàng
            var don = new DonHang
            {
                NgayDat = DateTime.Now,
                MaKH = maKH,
                MaGiaoHang = gh.MaGiaoHang,
                TrangThai = "Đang xử lý"
            };
            db.DonHangs.Add(don);
            db.SaveChanges();

            // 4. Thêm chi tiết đơn hàng, trừ kho
            foreach (var item in cart)
            {
                db.ChiTietDonHangs.Add(new ChiTietDonHang
                {
                    MaDonHang = don.MaDonHang,
                    MaSP = item.MaSP,
                    SoLuong = item.SoLuong
                });
                var sp = db.SanPhams.Find(item.MaSP);
                if (sp != null)
                    sp.SoLuong = (sp.SoLuong ?? 0) - item.SoLuong;
            }

            // 5. Xóa giỏ hàng
            db.GioHangs.RemoveRange(cart);
            db.SaveChanges();

            TempData["Message"] = "Đặt hàng thành công!";
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public ActionResult Checkout()
        {
            int maKH = Convert.ToInt32(Session["MaKH"]);
            var kh = db.KhachHangs.FirstOrDefault(x => x.MaKH == maKH);
            var model = new CheckoutViewModel
            {
                TenNguoiNhan = kh?.TenKH,
                SoDienThoai = kh?.SoDienThoai,
                DiaChiNhan = ""
            };
            return View(model);
        }

    }
}