using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using OfficeOpenXml;
using WebApplication1.Filters;
using WebApplication1.Models;
using WebApplication1.ViewModels;


namespace WebApplication1.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class DonHangController : Controller
    {
        private readonly QLCHQA db = new QLCHQA();

        // Danh sách đơn hàng + tìm kiếm + phân trang
        public ActionResult Index(string search, int page = 1)
        {
            const int pageSize = 10;
            var query = db.DonHangs.Include(d => d.KhachHang).Include(d => d.GiaoHang);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(d => d.KhachHang.TenKH.Contains(search));

            var total = query.Count();
            var paged = query.OrderByDescending(d => d.NgayDat)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

            // Chuyển sang ViewModel
            var items = paged.Select(d => new DonHangIndexViewModel
            {
                MaDonHang = d.MaDonHang,
                NgayDat = d.NgayDat ?? DateTime.MinValue,
                TenKH = d.KhachHang?.TenKH,
                DiaChiGiao = d.GiaoHang?.DiaChiNhan,
                TongTien = d.ChiTietDonHangs?.Sum(ct => (ct.SoLuong ?? 0) * Convert.ToDecimal(ct.SanPham.GiaBan ?? 0)) ?? 0,
                TongTienSauGiam = d.ChiTietDonHangs?.Sum(ct =>
                    (ct.SoLuong ?? 0)
                    * Convert.ToDecimal(ct.SanPham.GiaBan ?? 0)
                    * (1 - Convert.ToDecimal(ct.SanPham.GiamGia ?? 0) / 100)
                ) ?? 0,
                TrangThai = d.TrangThai
            }).ToList();

            var vm = new PagedListViewModel<DonHangIndexViewModel>
            {
                Items = items,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = total
            };
            return View(vm);
        }

        // Tạo đơn hàng GET
        public ActionResult Create(string searchSanPham = "")
        {
            var listSP = db.SanPhams
                .Where(sp => string.IsNullOrEmpty(searchSanPham) || sp.TenSP.Contains(searchSanPham))
                .Select(sp => new
                {
                    sp.MaSP,
                    sp.TenSP,
                    GiaBan = sp.GiaBan ?? 0,
                    GiamGia = sp.GiamGia ?? 0,
                    SoLuongTon = sp.SoLuong ?? 0
                })
                .ToList()
                .Select(sp => new SanPhamChonViewModel
                {
                    MaSP = sp.MaSP,
                    TenSP = sp.TenSP,
                    GiaBan = Convert.ToDecimal(sp.GiaBan),
                    GiamGia = Convert.ToDouble(sp.GiamGia),
                    SoLuongTon = Convert.ToInt32(sp.SoLuongTon),
                    SoLuong = 1,
                    IsChecked = false
                }).ToList();

            var vm = new DonHangCreateViewModel
            {
                NgayDat = DateTime.Now,
                MaDonHang = "DH" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                MaGiaoHang = "GH" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                SanPhams = listSP,
                SanPhamChons = new List<SanPhamChonViewModel>()
            };

            if (vm.SanPhams == null)
                vm.SanPhams = new List<SanPhamChonViewModel>();

            return View(vm);
        }

        // Tạo đơn hàng POST
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(DonHangCreateViewModel vm)
        {
            var spDebug = vm.SanPhamChons?.Count(x => x.SoLuong > 0);

            if (string.IsNullOrWhiteSpace(vm.TenKH))
                ModelState.AddModelError("TenKH", "Tên khách hàng bắt buộc");
            if (string.IsNullOrWhiteSpace(vm.DiaChiNhan))
                ModelState.AddModelError("DiaChiNhan", "Địa chỉ giao bắt buộc");

            if (vm.SanPhamChons == null) vm.SanPhamChons = new List<SanPhamChonViewModel>();


            var sanPhamChon = vm.SanPhamChons?.Where(x => x.IsChecked && x.SoLuong > 0).ToList();
            if (sanPhamChon == null || sanPhamChon.Count == 0)
            {
                ModelState.AddModelError("", "Phải chọn ít nhất một sản phẩm!");
                // Nạp lại danh sách SanPhams để trả về View nếu cần
                vm.SanPhams = db.SanPhams
                    .Select(sp => new
                    {
                        sp.MaSP,
                        sp.TenSP,
                        GiaBan = sp.GiaBan ?? 0,
                        GiamGia = sp.GiamGia ?? 0,
                        SoLuongTon = sp.SoLuong ?? 0
                    })
                    .ToList()
                    .Select(sp => new SanPhamChonViewModel
                    {
                        MaSP = sp.MaSP,
                        TenSP = sp.TenSP,
                        GiaBan = Convert.ToDecimal(sp.GiaBan),
                        GiamGia = Convert.ToDouble(sp.GiamGia),
                        SoLuongTon = Convert.ToInt32(sp.SoLuongTon),
                        SoLuong = 1
                    }).ToList();
                return View(vm);
            }

            foreach (var item in vm.SanPhamChons)
            {
                var sp = db.SanPhams.Find(item.MaSP);
                if (sp == null || (sp.SoLuong ?? 0) < item.SoLuong)
                    ModelState.AddModelError("", $"Sản phẩm {item.TenSP} không đủ tồn kho!");
            }

            // Load lại sản phẩm cho view nếu có lỗi
            if (!ModelState.IsValid)
            {
                var listSP = db.SanPhams
                    .Select(sp => new
                    {
                        sp.MaSP,
                        sp.TenSP,
                        GiaBan = sp.GiaBan ?? 0,
                        GiamGia = sp.GiamGia ?? 0,
                        SoLuongTon = sp.SoLuong ?? 0
                    })
                    .ToList()
                    .Select(sp => {
                        // Giữ lại trạng thái đã nhập trước đó nếu có
                        var spChon = vm.SanPhamChons?.FirstOrDefault(x => x.MaSP == sp.MaSP);
                        return new SanPhamChonViewModel
                        {
                            MaSP = sp.MaSP,
                            TenSP = sp.TenSP,
                            GiaBan = Convert.ToDecimal(sp.GiaBan),
                            GiamGia = Convert.ToDouble(sp.GiamGia),
                            SoLuongTon = Convert.ToInt32(sp.SoLuongTon),
                            SoLuong = spChon?.SoLuong ?? 1,
                            IsChecked = spChon?.IsChecked ?? false
                        };
                    }).ToList();

                vm.SanPhams = listSP;

                if (vm.SanPhams == null)
                    vm.SanPhams = new List<SanPhamChonViewModel>();
                if (vm.SanPhamChons == null)
                    vm.SanPhamChons = new List<SanPhamChonViewModel>();

                return View(vm);
            }


            // Thêm khách hàng mới
            var kh = new KhachHang
            {
                TenKH = vm.TenKH,
                Email = "",
                SoDienThoai = vm.SoDienThoai,
            };
            db.KhachHangs.Add(kh);
            db.SaveChanges();

            // Thêm địa chỉ giao (bảng GiaoHang)
            var gh = new GiaoHang
            {
                TenNguoiNhan = vm.TenKH,
                DiaChiNhan = vm.DiaChiNhan,
                SoDienThoai = vm.SoDienThoai,
                MaKH = kh.MaKH
            };
            db.GiaoHangs.Add(gh);
            db.SaveChanges();

            // Tạo đơn hàng
            var don = new DonHang
            {
                NgayDat = DateTime.Now,
                MaKH = kh.MaKH,
                MaGiaoHang = gh.MaGiaoHang,
                TrangThai = "Đang xử lý"
            };
            db.DonHangs.Add(don);
            db.SaveChanges();

            foreach (var item in vm.SanPhamChons.Where(x => x.SoLuong > 0))
            {
                db.ChiTietDonHangs.Add(new ChiTietDonHang
                {
                    MaDonHang = don.MaDonHang,
                    MaSP = item.MaSP,
                    SoLuong = item.SoLuong
                });
                // Trừ kho
                var sp = db.SanPhams.Find(item.MaSP);
                if (sp != null)
                    sp.SoLuong = (sp.SoLuong ?? 0) - item.SoLuong;
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Chi tiết đơn hàng
        public ActionResult Details(int? id)
        {
            if (id == null) return HttpNotFound();
            var d = db.DonHangs
                .Include(x => x.KhachHang)
                .Include(x => x.GiaoHang)
                .Include(x => x.ChiTietDonHangs.Select(ct => ct.SanPham))
                .FirstOrDefault(x => x.MaDonHang == id.Value);
            if (d == null) return HttpNotFound();
            return View(d);
        }

        // Xóa
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var d = db.DonHangs.Include(x => x.ChiTietDonHangs).FirstOrDefault(x => x.MaDonHang == id);
            if (d != null)
            {
                db.ChiTietDonHangs.RemoveRange(d.ChiTietDonHangs);
                db.DonHangs.Remove(d);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // Chuyển trạng thái
        [HttpPost]
        public ActionResult UpdateTrangThai(int id)
        {
            var don = db.DonHangs.Find(id);
            if (don == null) return HttpNotFound();
            if (don.TrangThai == "Đang xử lý")
                don.TrangThai = "Đang giao";
            else if (don.TrangThai == "Đang giao")
                don.TrangThai = "Hoàn thành";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Xuất Excel
        public ActionResult ExportExcel(string search)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var query = db.DonHangs.Include(d => d.KhachHang).Include(d => d.GiaoHang);
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(d => d.KhachHang.TenKH.Contains(search));
            var list = query.OrderByDescending(d => d.NgayDat).ToList();
            using (var pkg = new ExcelPackage())
            {
                var ws = pkg.Workbook.Worksheets.Add("Đơn hàng");
                for (int i = 0; i < list.Count; i++)
                {
                    var d = list[i];
                    ws.Cells[i + 2, 1].Value = d.MaDonHang;
                    ws.Cells[i + 2, 2].Value = d.NgayDat?.ToString("yyyy-MM-dd HH:mm");
                    ws.Cells[i + 2, 3].Value = d.KhachHang.TenKH;
                    ws.Cells[i + 2, 4].Value = d.GiaoHang?.DiaChiNhan;
                    ws.Cells[i + 2, 5].Value = d.ChiTietDonHangs.Sum(ct => (ct.SoLuong ?? 0) * Convert.ToDecimal(ct.SanPham.GiaBan ?? 0));
                    ws.Cells[i + 2, 6].Value = d.TrangThai;
                }
                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                var ms = new MemoryStream();
                pkg.SaveAs(ms);
                ms.Position = 0;
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"DonHang_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
