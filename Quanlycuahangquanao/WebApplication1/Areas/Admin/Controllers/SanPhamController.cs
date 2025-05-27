using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using OfficeOpenXml;                            // EPPlus core
using WebApplication1.Filters;
using WebApplication1.Models;                  // SanPham, DanhMuc, QLCHQA
using WebApplication1.ViewModels;              // ProductIndexViewModel, ProductCreateEditViewModel, PagedListViewModel<T>

namespace WebApplication1.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class SanPhamController : Controller
    {
        private readonly QLCHQA db = new QLCHQA();

        // GET: Admin/SanPham
        public ActionResult Index(string search, int page = 1)
        {
            const int pageSize = 10;
            var query = db.SanPhams.Include(sp => sp.DanhMucs).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(sp => sp.TenSP.Contains(search));

            var total = query.Count();
            var items = query
                .OrderBy(sp => sp.MaSP)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(sp => new ProductIndexViewModel
                {
                    MaSP = sp.MaSP,
                    HinhAnh = sp.HinhAnh,
                    TenSP = sp.TenSP,
                    SoLuong = sp.SoLuong ?? 0,
                    GiamGia = sp.GiamGia ?? 0,
                    GiaBan = sp.GiaBan ?? 0,
                    TrangThai = (sp.TrangThai == "True") ? "Mở bán" : "Ngưng bán",
                    LaSanPhamMoi = sp.LaSanPhamMoi ?? false
                })
                .ToList();

            var vm = new PagedListViewModel<ProductIndexViewModel>
            {
                Items = items,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = total
            };
            ViewBag.Search = search;
            return View(vm);
        }

        // GET: Admin/SanPham/ExportExcel
        public ActionResult ExportExcel(string search)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var query = db.SanPhams.Include(sp => sp.DanhMucs).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(sp => sp.TenSP.Contains(search));
            var list = query.OrderBy(sp => sp.MaSP).ToList();

            using (var pkg = new ExcelPackage())
            {
                var ws = pkg.Workbook.Worksheets.Add("Sản phẩm");
                // Header
                ws.Cells[1, 1].Value = "Mã SP";
                ws.Cells[1, 2].Value = "Tên SP";
                ws.Cells[1, 3].Value = "Danh mục";
                ws.Cells[1, 4].Value = "Số lượng";
                ws.Cells[1, 5].Value = "Khuyến mãi (%)";
                ws.Cells[1, 6].Value = "Giá bán";
                ws.Cells[1, 7].Value = "Trạng thái";
                ws.Cells[1, 8].Value = "Mới";
                ws.Cells[1, 9].Value = "Mô tả";
                ws.Cells[1, 10].Value = "Kích thước";

                for (int i = 0; i < list.Count; i++)
                {
                    var sp = list[i];
                    ws.Cells[i + 2, 1].Value = sp.MaSP;
                    ws.Cells[i + 2, 2].Value = sp.TenSP;
                    ws.Cells[i + 2, 3].Value = string.Join(", ",
                        sp.DanhMucs.Select(dm => dm.TenDanhMuc));
                    ws.Cells[i + 2, 4].Value = sp.SoLuong ?? 0;
                    ws.Cells[i + 2, 5].Value = sp.GiamGia ?? 0;
                    ws.Cells[i + 2, 6].Value = sp.GiaBan ?? 0;
                    ws.Cells[i + 2, 7].Value = (sp.TrangThai == "True")
                                              ? "Mở bán" : "Ngưng bán";
                    ws.Cells[i + 2, 8].Value = (sp.LaSanPhamMoi == true)
                                              ? "Có" : "Không";
                    ws.Cells[i + 2, 9].Value = sp.MoTa;
                    ws.Cells[i + 2, 10].Value = sp.KichThuoc;
                }

                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                var ms = new MemoryStream();
                pkg.SaveAs(ms);
                ms.Position = 0;
                string fileName = $"SanPham_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(
                    ms,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                );
            }
        }

        // GET: Admin/SanPham/Create
        [HttpGet]
        public ActionResult Create()
        {
            var vm = new ProductCreateEditViewModel
            {
                AllCategories = db.DanhMucs
                    .OrderBy(dm => dm.TenDanhMuc)
                    .Select(dm => new SelectListItem
                    {
                        Value = dm.MaDanhMuc.ToString(),
                        Text = dm.TenDanhMuc
                    })
                    .ToList(),
                TrangThai = "True"  // mặc định mở bán
            };
            return View(vm);
        }

        // POST: Admin/SanPham/Create
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(ProductCreateEditViewModel vm, HttpPostedFileBase ImageFile)
        {
            if (vm.SelectedCategoryIds == null || !vm.SelectedCategoryIds.Any())
                ModelState.AddModelError(
                    nameof(vm.SelectedCategoryIds),
                    "Phải chọn ít nhất 1 danh mục.");

            if (!ModelState.IsValid)
            {
                vm.AllCategories = db.DanhMucs
                    .OrderBy(dm => dm.TenDanhMuc)
                    .Select(dm => new SelectListItem
                    {
                        Value = dm.MaDanhMuc.ToString(),
                        Text = dm.TenDanhMuc
                    })
                    .ToList();
                return View(vm);
            }

            var sp = new SanPham
            {
                TenSP = vm.TenSP,
                SoLuong = vm.SoLuong,
                GiamGia = vm.GiamGia,
                GiaBan = vm.GiaBan,
                MoTa = vm.MoTa,
                KichThuoc = vm.KichThuoc,
                TrangThai = vm.TrangThai,
                LaSanPhamMoi = vm.LaSanPhamMoi ?? (false)
            };

            // **Lưu file ảnh**
            if (ImageFile != null && ImageFile.ContentLength > 0)
            {
                var fileName = System.IO.Path.GetFileName(ImageFile.FileName);
                var path = Server.MapPath("~/Assets/images/" + fileName);
                ImageFile.SaveAs(path);

                // Lưu đường dẫn vào DB (tốt nhất chỉ lưu tên file)
                sp.HinhAnh = "/Assets/images/" + fileName;
            }


            db.SanPhams.Add(sp);
            db.SaveChanges();
            TempData["Success"] = "Thêm sản phẩm thành công!";
            return RedirectToAction("Index");
        }

        // GET: Admin/SanPham/Edit/5
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var p = db.SanPhams
                      .Include(sp => sp.DanhMucs)
                      .FirstOrDefault(sp => sp.MaSP == id.Value);
            if (p == null) return HttpNotFound();

            var vm = new ProductCreateEditViewModel
            {
                MaSP = p.MaSP,
                TenSP = p.TenSP,
                SoLuong = p.SoLuong ?? 0,
                GiamGia = p.GiamGia ?? 0,
                GiaBan = p.GiaBan ?? 0,
                MoTa = p.MoTa,
                KichThuoc = p.KichThuoc,
                TrangThai = p.TrangThai,
                LaSanPhamMoi = p.LaSanPhamMoi,
                HinhAnhHienTai = p.HinhAnh,
                SelectedCategoryIds = p.DanhMucs.Select(dm => dm.MaDanhMuc).ToArray(),
                AllCategories = db.DanhMucs
                    .OrderBy(dm => dm.TenDanhMuc)
                    .Select(dm => new SelectListItem
                    {
                        Value = dm.MaDanhMuc.ToString(),
                        Text = dm.TenDanhMuc
                    })
                    .ToList()
            };
            return View(vm);
        }

        // POST: Admin/SanPham/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(ProductCreateEditViewModel vm, HttpPostedFileBase ImageFile)
        {
            if (vm.SelectedCategoryIds == null || !vm.SelectedCategoryIds.Any())
                ModelState.AddModelError(nameof(vm.SelectedCategoryIds),
                    "Phải chọn ít nhất 1 danh mục.");

            if (!ModelState.IsValid)
            {
                vm.AllCategories = db.DanhMucs
                    .OrderBy(dm => dm.TenDanhMuc)
                    .Select(dm => new SelectListItem
                    {
                        Value = dm.MaDanhMuc.ToString(),
                        Text = dm.TenDanhMuc
                    })
                    .ToList();
                return View(vm);
            }

            var p = db.SanPhams
                      .Include(sp => sp.DanhMucs)
                      .FirstOrDefault(sp => sp.MaSP == vm.MaSP);
            if (p == null) return HttpNotFound();

            p.TenSP = vm.TenSP;
            p.SoLuong = vm.SoLuong;
            p.GiamGia = vm.GiamGia;
            p.GiaBan = vm.GiaBan;
            p.MoTa = vm.MoTa;
            p.KichThuoc = vm.KichThuoc;
            p.TrangThai = vm.TrangThai;
            p.LaSanPhamMoi = vm.LaSanPhamMoi ?? (false);
            if (vm.ImageFile?.ContentLength > 0)
            {
                var fn = $"{Guid.NewGuid()}_{Path.GetFileName(vm.ImageFile.FileName)}";
                var path = Server.MapPath("~/Assets/images/" + fn);
                vm.ImageFile.SaveAs(path);
                p.HinhAnh = "/Assets/images/" + fn;
            }

            // Xử lý file ảnh mới upload (nếu có)
            if (ImageFile != null && ImageFile.ContentLength > 0)
            {
                var fn = $"{Guid.NewGuid()}_{Path.GetFileName(ImageFile.FileName)}";
                var path = Server.MapPath("~/Assets/images/" + fn);
                ImageFile.SaveAs(path);
                p.HinhAnh = "/Assets/images/" + fn;
            }

            p.DanhMucs.Clear();
            foreach (var catId in vm.SelectedCategoryIds)
            {
                var dm = db.DanhMucs.Find(catId);
                if (dm != null)
                    p.DanhMucs.Add(dm);
            }

            db.SaveChanges();
            TempData["Success"] = "Cập nhật thành công!";
            return RedirectToAction("Index");
        }

        // GET: Admin/SanPham/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var sp = db.SanPhams
                       .Include(s => s.DanhMucs)
                       .FirstOrDefault(s => s.MaSP == id.Value);
            if (sp == null) return HttpNotFound();
            return View(sp);
        }

        // GET: Admin/SanPham/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Load luôn quan hệ DanhMucs
            var sp = db.SanPhams
                       .Include(s => s.DanhMucs)
                       .FirstOrDefault(s => s.MaSP == id);
            if (sp == null) return HttpNotFound();

            // Xóa hết liên kết với các danh mục
            sp.DanhMucs.Clear();
            db.SaveChanges();  // commit xóa trong bảng trung gian

            // Bây giờ mới xóa sản phẩm
            db.SanPhams.Remove(sp);
            db.SaveChanges();

            TempData["Success"] = "Xóa sản phẩm thành công!";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
