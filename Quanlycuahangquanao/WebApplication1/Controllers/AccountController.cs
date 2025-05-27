using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private QLCHQA db = new QLCHQA();

        // Đăng ký
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string Email, string SoDienThoai, string MatKhau, string TenKH)
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(SoDienThoai)
                || string.IsNullOrWhiteSpace(MatKhau) || string.IsNullOrWhiteSpace(TenKH))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
                return View();
            }

            // Check trùng email
            if (db.KhachHangs.Any(x => x.Email == Email))
            {
                ViewBag.Error = "Email này đã được đăng ký!";
                return View();
            }
            // Check trùng SĐT
            if (db.KhachHangs.Any(x => x.SoDienThoai == SoDienThoai))
            {
                ViewBag.Error = "Số điện thoại này đã được đăng ký!";
                return View();
            }

            // Tạo mới khách hàng
            var kh = new KhachHang
            {
                TenKH = TenKH,
                Email = Email,
                SoDienThoai = SoDienThoai
            };
            db.KhachHangs.Add(kh);
            db.SaveChanges();

            // Tạo tài khoản (lấy Email làm tên đăng nhập)
            var tk = new TaiKhoan
            {
                TenDangNhap = Email,
                MatKhau = MatKhau,
                NgayTao = DateTime.Now,
                TrangThai = "active",
                MaLoaiTK = 3, // Khách hàng
                MaKH = kh.MaKH
            };
            db.TaiKhoans.Add(tk);
            db.SaveChanges();

            TempData["Success"] = "Đăng ký thành công! Bạn có thể đăng nhập ngay.";
            return RedirectToAction("Login");
        }
        // Đăng nhập
        [HttpGet]
        public ActionResult Login()
        {
            // Nhận thông báo đăng ký thành công nếu có
            ViewBag.Success = TempData["Success"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Email, string MatKhau)
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(MatKhau))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
                return View();
            }

            // Đăng nhập bằng email
            var user = db.TaiKhoans
                .FirstOrDefault(u => u.TenDangNhap == Email && u.MatKhau == MatKhau && u.TrangThai == "active");
            if (user == null)
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
                return View();
            }
            Session["MaTK"] = user.MaTK;
            Session["TenDangNhap"] = user.TenDangNhap;
            Session["MaLoaiTK"] = user.MaLoaiTK;
            Session["MaKH"] = user.MaKH;
            return RedirectToAction("Index", "Home");
        }


        // Quên mật khẩu
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string EmailOrPhone)
        {
            if (string.IsNullOrWhiteSpace(EmailOrPhone))
            {
                ViewBag.Error = "Bạn chưa nhập email hoặc số điện thoại!";
                return View();
            }

            KhachHang kh = null;

            if (EmailOrPhone.Contains("@"))
                kh = db.KhachHangs.FirstOrDefault(x => x.Email == EmailOrPhone);
            else
                kh = db.KhachHangs.FirstOrDefault(x => x.SoDienThoai == EmailOrPhone);

            if (kh == null)
            {
                ViewBag.Error = "Không tìm thấy tài khoản với thông tin này!";
                return View();
            }

            var tk = db.TaiKhoans.FirstOrDefault(t => t.MaKH == kh.MaKH);
            if (tk == null)
            {
                ViewBag.Error = "Không tìm thấy tài khoản với thông tin này!";
                return View();
            }

            // Tạo mật khẩu mới
            string newPassword = "123456"; // Hoặc random
            tk.MatKhau = newPassword;
            db.SaveChanges();

            ViewBag.Success = "Mật khẩu mới đã được cập nhật! (Liên hệ quản trị hoặc kiểm tra email nếu có cấu hình)";
            return View();
        }

        // Đổi mật khẩu
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string oldPassword, string newPassword)
        {
            if (Session["MaTK"] == null)
            {
                return RedirectToAction("Login");
            }
            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
                return View();
            }
            int id = (int)Session["MaTK"];
            var user = db.TaiKhoans.Find(id);
            if (user != null && user.MatKhau == oldPassword)
            {
                user.MatKhau = newPassword;
                db.SaveChanges();
                ViewBag.Success = "Đổi mật khẩu thành công!";
            }
            else
            {
                ViewBag.Error = "Mật khẩu cũ không đúng!";
            }
            return View();
        }

        // Đăng xuất
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // Trang hiển thị thông tin tài khoản
        [HttpGet]
        public ActionResult Profile()
        {
            if (Session["MaKH"] == null) return RedirectToAction("Login");

            int maKH = Convert.ToInt32(Session["MaKH"]);
            var kh = db.KhachHangs.Find(maKH);
            if (kh == null) return RedirectToAction("Login");
            return View(kh);
        }

        // Trang cập nhật thông tin
        [HttpGet]
        public ActionResult EditProfile()
        {
            if (Session["MaKH"] == null) return RedirectToAction("Login");

            int maKH = Convert.ToInt32(Session["MaKH"]);
            var kh = db.KhachHangs.Find(maKH);
            if (kh == null) return RedirectToAction("Login");
            return View(kh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(HttpPostedFileBase AnhDaiDien, string TenKH, string Email, string SoDienThoai)
        {
            if (Session["MaKH"] == null) return RedirectToAction("Login");

            int maKH = Convert.ToInt32(Session["MaKH"]);
            var kh = db.KhachHangs.Find(maKH);
            if (kh == null) return RedirectToAction("Login");

            kh.TenKH = TenKH;
            kh.Email = Email;
            kh.SoDienThoai = SoDienThoai;

            // Đổi ảnh đại diện (nếu có upload)
            if (AnhDaiDien != null && AnhDaiDien.ContentLength > 0)
            {
                string fileName = Guid.NewGuid() + System.IO.Path.GetExtension(AnhDaiDien.FileName);
                string path = Server.MapPath("~/Assets/Avatar/" + fileName);
                AnhDaiDien.SaveAs(path);
                kh.AnhDaiDien = "/Assets/Avatar/" + fileName;
            }

            db.SaveChanges();
            ViewBag.Success = "Cập nhật thành công!";
            return RedirectToAction("Profile");
        }
    }

}

