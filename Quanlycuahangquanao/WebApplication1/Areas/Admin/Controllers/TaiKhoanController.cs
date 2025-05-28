using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.ViewModels;
using OfficeOpenXml;
using System.IO;
using WebApplication1.Filters;
using System.Web;

namespace WebApplication1.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class TaiKhoanController : Controller
    {
        private readonly QLCHQA db = new QLCHQA();

        // List, Search, Paging
        public ActionResult Index(string search, int page = 1)
        {
            const int pageSize = 10;
            var query = db.TaiKhoans.Include(t => t.LoaiTaiKhoan).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t => t.TenDangNhap.Contains(search) || t.LoaiTaiKhoan.TenLoaiTK.Contains(search));
            }
            var total = query.Count();
            var items = query.OrderBy(t => t.MaTK)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TaiKhoanIndexViewModel
                {
                    MaTK = t.MaTK,
                    TenDangNhap = t.TenDangNhap,
                    MatKhau = t.MatKhau,
                    TenLoaiTK = t.LoaiTaiKhoan.TenLoaiTK
                }).ToList();

            var vm = new PagedListViewModel<TaiKhoanIndexViewModel>
            {
                Items = items,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = total
            };
            ViewBag.Search = search;
            return View(vm);
        }

        // Export Excel
        public ActionResult ExportExcel(string search)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var query = db.TaiKhoans.Include(t => t.LoaiTaiKhoan).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t => t.TenDangNhap.Contains(search) || t.LoaiTaiKhoan.TenLoaiTK.Contains(search));
            }
            var list = query.OrderBy(t => t.MaTK).ToList();

            using (var pkg = new ExcelPackage())
            {
                var ws = pkg.Workbook.Worksheets.Add("TaiKhoan");
                ws.Cells[1, 1].Value = "Mã TK";
                ws.Cells[1, 2].Value = "Tài khoản (Email)";
                ws.Cells[1, 3].Value = "Mật khẩu";
                ws.Cells[1, 4].Value = "Loại tài khoản";
                for (int i = 0; i < list.Count; i++)
                {
                    var tk = list[i];
                    ws.Cells[i + 2, 1].Value = tk.MaTK;
                    ws.Cells[i + 2, 2].Value = tk.TenDangNhap;
                    ws.Cells[i + 2, 3].Value = tk.MatKhau;
                    ws.Cells[i + 2, 4].Value = tk.LoaiTaiKhoan?.TenLoaiTK ?? "";
                }
                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                var ms = new MemoryStream();
                pkg.SaveAs(ms);
                ms.Position = 0;
                return File(ms,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"TaiKhoan_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }
        }

        // Details
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var t = db.TaiKhoans.Include(x => x.LoaiTaiKhoan).FirstOrDefault(x => x.MaTK == id.Value);
            if (t == null) return HttpNotFound();

            var vm = new TaiKhoanDetailsViewModel
            {
                MaTK = t.MaTK,
                TenDangNhap = t.TenDangNhap,
                MatKhau = t.MatKhau,
                TenLoaiTK = t.LoaiTaiKhoan?.TenLoaiTK,
                NgayTao = t.NgayTao,
                TrangThai = t.TrangThai
            };
            return View(vm);
        }

        // GET: Create
        public ActionResult Create()
        {
            var model = new TaiKhoanCreateViewModel
            {
                ListLoaiTK = db.LoaiTaiKhoans.OrderBy(l => l.TenLoaiTK).Select(l => new SelectListItem
                {
                    Value = l.MaLoaiTK.ToString(),
                    Text = l.TenLoaiTK
                })
            };
            return View(model);
        }

        // POST: Create
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(TaiKhoanCreateViewModel vm, string MatKhauConfirm)
        {
            vm.ListLoaiTK = db.LoaiTaiKhoans.OrderBy(l => l.TenLoaiTK).Select(l => new SelectListItem
            {
                Value = l.MaLoaiTK.ToString(),
                Text = l.TenLoaiTK
            });
            // Check validate
            if (!ModelState.IsValid) return View(vm);

            if (db.TaiKhoans.Any(x => x.TenDangNhap == vm.Email))
            {
                ViewBag.Error = "Email đã tồn tại!";
                return View(vm);
            }
            if (vm.MatKhau != MatKhauConfirm)
            {
                ViewBag.ConfirmError = "Mật khẩu nhập lại không khớp!";
                return View(vm);
            }
            var tk = new TaiKhoan
            {
                TenDangNhap = vm.Email,
                MatKhau = vm.MatKhau,
                MaLoaiTK = vm.MaLoaiTK,
                NgayTao = DateTime.Now,
                TrangThai = "Hoạt động"
            };
            db.TaiKhoans.Add(tk);
            db.SaveChanges();
            TempData["Success"] = "Tạo tài khoản thành công!";
            return RedirectToAction("Index");
        }

        // GET: Edit
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var t = db.TaiKhoans.Find(id);
            if (t == null) return HttpNotFound();

            var model = new TaiKhoanEditViewModel
            {
                MaTK = t.MaTK,
                Email = t.TenDangNhap,
                MatKhau = t.MatKhau,
                MaLoaiTK = t.MaLoaiTK,
                ListLoaiTK = db.LoaiTaiKhoans.OrderBy(l => l.TenLoaiTK).Select(l => new SelectListItem
                {
                    Value = l.MaLoaiTK.ToString(),
                    Text = l.TenLoaiTK
                })
            };
            return View(model);
        }

        // POST: Edit
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(TaiKhoanEditViewModel vm)
        {
            vm.ListLoaiTK = db.LoaiTaiKhoans.OrderBy(l => l.TenLoaiTK).Select(l => new SelectListItem
            {
                Value = l.MaLoaiTK.ToString(),
                Text = l.TenLoaiTK
            });
            if (!ModelState.IsValid) return View(vm);

            var tk = db.TaiKhoans.Find(vm.MaTK);
            if (tk == null) return HttpNotFound();

            if (db.TaiKhoans.Any(x => x.TenDangNhap == vm.Email && x.MaTK != vm.MaTK))
            {
                ViewBag.Error = "Email đã tồn tại!";
                return View(vm);
            }

            tk.TenDangNhap = vm.Email;
            if (!string.IsNullOrEmpty(vm.MatKhau)) tk.MatKhau = vm.MatKhau;
            tk.MaLoaiTK = vm.MaLoaiTK;
            db.SaveChanges();
            TempData["Success"] = "Cập nhật tài khoản thành công!";
            return RedirectToAction("Index");
        }

        // POST: Delete
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var t = db.TaiKhoans.Find(id);
            if (t != null)
            {
                db.TaiKhoans.Remove(t);
                db.SaveChanges();
                TempData["Success"] = "Xóa tài khoản thành công!";
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }

        // Đăng xuất
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();

            // (Nếu có cookie đăng nhập thì xóa luôn)
            if (Request.Cookies["auth"] != null)
            {
                var c = new HttpCookie("auth");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
            }
            return RedirectToAction("Login", "Account", new { area = "" });
        }
    }
}
