using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using WebApplication1.Filters;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class KhachHangController : Controller
    {
        private readonly QLCHQA db = new QLCHQA();

        // Danh sách + tìm kiếm + phân trang
        public ActionResult Index(string search, int page = 1)
        {
            const int pageSize = 10;
            var query = db.KhachHangs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(k => k.TenKH.Contains(search)
                                       || k.SoDienThoai.Contains(search)
                                       || k.Email.Contains(search));
            }

            var total = query.Count();
            var paged = query.OrderBy(k => k.MaKH)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .AsEnumerable();

            var items = paged
                .Select(k => new KhachHangIndexViewModel
                {
                    MaKH = k.MaKH,
                    TenKH = k.TenKH,
                    SoDienThoai = k.SoDienThoai,
                    Email = k.Email,
                    AnhDaiDien = k.AnhDaiDien
                }).ToList();

            var vm = new PagedListViewModel<KhachHangIndexViewModel>
            {
                Items = items,
                CurrentPage = page,
                TotalItems = total,
            };
            ViewBag.Search = search;
            return View(vm);
        }

        // Xuất Excel
        public ActionResult ExportExcel(string search)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var query = db.KhachHangs.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(k => k.TenKH.Contains(search)
                                     || k.SoDienThoai.Contains(search)
                                     || k.Email.Contains(search));

            var list = query.OrderBy(k => k.MaKH).ToList();

            using (var pkg = new ExcelPackage())
            {
                var ws = pkg.Workbook.Worksheets.Add("KhachHang");
                ws.Cells[1, 1].Value = "Mã KH";
                ws.Cells[1, 2].Value = "Tên KH";
                ws.Cells[1, 3].Value = "Số điện thoại";
                ws.Cells[1, 4].Value = "Email";
                ws.Cells[1, 5].Value = "Ảnh đại diện";

                for (int i = 0; i < list.Count; i++)
                {
                    var k = list[i];
                    ws.Cells[i + 2, 1].Value = k.MaKH;
                    ws.Cells[i + 2, 2].Value = k.TenKH;
                    ws.Cells[i + 2, 3].Value = k.SoDienThoai;
                    ws.Cells[i + 2, 4].Value = k.Email;
                    ws.Cells[i + 2, 5].Value = k.AnhDaiDien;
                }
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                var ms = new MemoryStream();
                pkg.SaveAs(ms);
                ms.Position = 0;
                return File(ms,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"KhachHang_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }
        }

        // Details
        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var k = db.KhachHangs.Find(id);
            if (k == null) return HttpNotFound();

            var vm = new KhachHangDetailsViewModel
            {
                MaKH = k.MaKH,
                TenKH = k.TenKH,
                SoDienThoai = k.SoDienThoai,
                Email = k.Email,
                AnhDaiDien = k.AnhDaiDien
            };
            return View(vm);
        }

        // Tạo mới - GET
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // Tạo mới - POST
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(KhachHangCreateViewModel vm, HttpPostedFileBase AnhDaiDienFile)
        {
            if (!ModelState.IsValid)
                return View(vm);

            if (db.KhachHangs.Any(k => k.SoDienThoai == vm.SoDienThoai))
            {
                ViewBag.Error = "Số điện thoại đã tồn tại!";
                return View(vm);
            }
            if (db.KhachHangs.Any(k => k.Email == vm.Email))
            {
                ViewBag.Error = "Email đã tồn tại!";
                return View(vm);
            }

            var kh = new KhachHang
            {
                TenKH = vm.TenKH,
                SoDienThoai = vm.SoDienThoai,
                Email = vm.Email
            };

            if (AnhDaiDienFile != null && AnhDaiDienFile.ContentLength > 0)
            {
                string fileName = Path.GetFileName(AnhDaiDienFile.FileName);
                string path = Path.Combine(Server.MapPath("~/Asset/images"), fileName);
                AnhDaiDienFile.SaveAs(path);
                kh.AnhDaiDien = fileName;
            }

            db.KhachHangs.Add(kh);
            db.SaveChanges();
            TempData["Success"] = "Thêm khách hàng thành công!";
            return RedirectToAction("Index");
        }

        // Sửa - GET
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var k = db.KhachHangs.Find(id);
            if (k == null) return HttpNotFound();

            var vm = new KhachHangEditViewModel
            {
                MaKH = k.MaKH,
                TenKH = k.TenKH,
                SoDienThoai = k.SoDienThoai,
                Email = k.Email,
                AnhDaiDien = k.AnhDaiDien
            };
            return View(vm);
        }

        // Sửa - POST
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(KhachHangEditViewModel vm, HttpPostedFileBase AnhDaiDienFile)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var kh = db.KhachHangs.Find(vm.MaKH);
            if (kh == null) return HttpNotFound();

            if (db.KhachHangs.Any(k => k.SoDienThoai == vm.SoDienThoai && k.MaKH != vm.MaKH))
            {
                ViewBag.Error = "Số điện thoại đã tồn tại!";
                return View(vm);
            }
            if (db.KhachHangs.Any(k => k.Email == vm.Email && k.MaKH != vm.MaKH))
            {
                ViewBag.Error = "Email đã tồn tại!";
                return View(vm);
            }

            kh.TenKH = vm.TenKH;
            kh.SoDienThoai = vm.SoDienThoai;
            kh.Email = vm.Email;

            if (AnhDaiDienFile != null && AnhDaiDienFile.ContentLength > 0)
            {
                string fileName = Path.GetFileName(AnhDaiDienFile.FileName);
                string path = Path.Combine(Server.MapPath("~/Asset/images"), fileName);
                AnhDaiDienFile.SaveAs(path);
                kh.AnhDaiDien = fileName;
            }

            db.SaveChanges();
            TempData["Success"] = "Cập nhật thành công!";
            return RedirectToAction("Index");
        }

        // Xóa
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var k = db.KhachHangs.Find(id);
            if (k != null)
            {
                db.KhachHangs.Remove(k);
                db.SaveChanges();
                TempData["Success"] = "Xóa thành công!";
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
