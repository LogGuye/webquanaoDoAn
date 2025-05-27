using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using WebApplication1.ViewModels;
using WebApplication1.Models;
using WebApplication1.Filters;

namespace WebApplication1.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class DanhMucController : Controller
    {
        private readonly QLCHQA db = new QLCHQA();

        // GET: Admin/DanhMuc
        public ActionResult Index(int page = 1, string search = "")
        {
            const int pageSize = 10;

            // 1) Chuẩn bị query gốc
            var query = db.DanhMucs.AsQueryable();

            // 2) Nếu có search thì lọc
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(dm => dm.TenDanhMuc.Contains(search));
            }

            // 3) Đếm tổng
            var totalCount = query.Count();

            // 4) Lấy page hiện tại và project về ViewModel
            var items = query
                .OrderBy(dm => dm.MaDanhMuc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(dm => new DanhMucIndexViewModel
                {
                    MaDanhMuc = dm.MaDanhMuc,
                    TenDanhMuc = dm.TenDanhMuc,
                    // Đếm số sản phẩm qua navigation property
                    ProductCount = dm.SanPhams.Count()
                })
                .ToList();

            // 5) Đẩy vào PagedListViewModel
            var vm = new PagedListViewModel<DanhMucIndexViewModel>
            {
                Items = items,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalCount
            };

            return View(vm);
        }

        // GET: Admin/DanhMuc/ExportToExcel
        public ActionResult ExportToExcel()
        {
            // EPPlus >= 5+: thiết lập license context (chỉ cần 1 lần, ví dụ ở Application_Start)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var pack = new ExcelPackage())
            {
                var ws = pack.Workbook.Worksheets.Add("DanhMuc");

                // 1) Header
                ws.Cells[1, 1].Value = "Mã danh mục";
                ws.Cells[1, 2].Value = "Tên danh mục";
                ws.Cells[1, 3].Value = "Số sản phẩm";

                // 2) Dữ liệu
                int row = 2;
                var all = db.DanhMucs
                            .OrderBy(dm => dm.MaDanhMuc)
                            .ToList(); // bring into memory so we can use navigation

                foreach (var dm in all)
                {
                    ws.Cells[row, 1].Value = dm.MaDanhMuc;
                    ws.Cells[row, 2].Value = dm.TenDanhMuc;
                    // đếm bằng navigation property
                    ws.Cells[row, 3].Value = dm.SanPhams.Count;
                    row++;
                }

                // 3) Định dạng (tùy chọn)
                ws.Cells[1, 1, 1, 3].Style.Font.Bold = true;
                ws.Cells.AutoFitColumns();

                // 4) Xuất file
                using (var stream = new MemoryStream())
                {
                    pack.SaveAs(stream);
                    stream.Position = 0;
                    var fileName = $"DanhMuc_{DateTime.Now:yyyyMMdd}.xlsx";
                    return File(
                        stream,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName
                    );
                }
            }
        }
        // GET: Admin/DanhMuc/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/DanhMuc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DanhMuc model)
        {
            if (db.DanhMucs.Any(dm => dm.TenDanhMuc == model.TenDanhMuc))
                ModelState.AddModelError(nameof(model.TenDanhMuc), "Tên danh mục đã tồn tại.");

            if (!ModelState.IsValid)
                return View(model);

            db.DanhMucs.Add(model);
            db.SaveChanges();
            TempData["Success"] = "Tạo danh mục thành công.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/DanhMuc/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var entity = db.DanhMucs.Find(id);
            if (entity == null) return HttpNotFound();

            var vm = new DanhMucCreateEditViewModel
            {
                MaDanhMuc = entity.MaDanhMuc,
                TenDanhMuc = entity.TenDanhMuc
            };
            return View(vm);
        }

        // POST: Admin/DanhMuc/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(DanhMucCreateEditViewModel vm)
        {
            // Check trùng tên (ngoại trừ chính nó)
            if (db.DanhMucs.Any(d => d.TenDanhMuc == vm.TenDanhMuc && d.MaDanhMuc != vm.MaDanhMuc))
                ModelState.AddModelError(nameof(vm.TenDanhMuc), "Tên danh mục đã tồn tại.");

            if (!ModelState.IsValid)
                return View(vm);

            var entity = db.DanhMucs.Find(vm.MaDanhMuc);
            if (entity == null) return HttpNotFound();

            entity.TenDanhMuc = vm.TenDanhMuc;
            db.SaveChanges();
            TempData["Success"] = "Cập nhật thành công danh mục “" + vm.TenDanhMuc + "”.";
            return RedirectToAction(nameof(Index));
        }


        // POST: Admin/DanhMuc/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var dm = db.DanhMucs.Find(id);
            if (dm != null)
            {
                db.DanhMucs.Remove(dm);
                db.SaveChanges();
                TempData["Success"] = "Xóa danh mục thành công.";
            }
            return RedirectToAction(nameof(Index));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
