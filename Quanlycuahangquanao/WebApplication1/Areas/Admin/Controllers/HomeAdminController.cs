using System;
using System.Linq;
using System.Web.Mvc;
using WebApplication1.ViewModels;
using WebApplication1.Models;
using WebApplication1.Filters;

namespace WebApplication1.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class HomeAdminController : Controller
    {
        private readonly QLCHQA db = new QLCHQA();

        // GET: Admin/HomeAdmin
        public ActionResult Index()
        {
            var now = DateTime.Now;

            // Tổng doanh thu của tất cả các đơn hoàn thành
            double? totalRevNullable = db.ChiTietDonHangs
                .Where(od => od.DonHang.TrangThai == "Hoàn thành")
                .Sum(od => (double?)(od.SoLuong *
                       (od.SanPham.GiaBan - od.SanPham.GiaBan * od.SanPham.GiamGia / 100)));

            // Doanh thu tháng này
            double? monthlyRevNullable = db.ChiTietDonHangs
                .Where(od => od.DonHang.TrangThai == "Hoàn thành"
                          && od.DonHang.NgayDat.HasValue
                          && od.DonHang.NgayDat.Value.Year == now.Year
                          && od.DonHang.NgayDat.Value.Month == now.Month)
                .Sum(od => (double?)(od.SoLuong *
                       (od.SanPham.GiaBan - od.SanPham.GiaBan * od.SanPham.GiamGia / 100)));

            // Thống kê 4 năm gần nhất
            var revenueByYear = db.ChiTietDonHangs
                                  .Where(od => od.DonHang.TrangThai == "Hoàn thành"
                                            && od.DonHang.NgayDat.HasValue)
                                  .GroupBy(od => od.DonHang.NgayDat.Value.Year)
                                  .OrderByDescending(g => g.Key)
                                  .Take(4)
                                  .ToDictionary(
                                     g => g.Key,
                                     g => g.Sum(od => od.SoLuong * (od.SanPham.GiaBan - od.SanPham.GiaBan * od.SanPham.GiamGia / 100))
                                  );

            // Khách hàng
            var totalCust = db.KhachHangs.Count();
            var monthlyCust = db.KhachHangs.Count(c => c.Email != null && c.MaKH > 0
                                    && c.MaKH != 0
                                    && c.MaKH != 0); // giả sử filter theo ngày tạo nếu có field

            // Đơn hàng
            var totalOrd = db.DonHangs.Count();
            var monthlyOrd = db.DonHangs.Count(o => o.NgayDat.HasValue
                                     && o.NgayDat.Value.Year == now.Year
                                     && o.NgayDat.Value.Month == now.Month);

            // Sản phẩm theo danh mục (chủ đề)
            var catCounts = db.DanhMucs
                              .Select(dm => new
                              {
                                  dm.TenDanhMuc,
                                  Count = dm.SanPhams.Count()
                              })
                              .ToDictionary(x => x.TenDanhMuc, x => x.Count);

            var vm = new AdminDashboardViewModel
            {
                TotalRevenue = totalRevNullable.GetValueOrDefault(),
                MonthlyRevenue = monthlyRevNullable.GetValueOrDefault(),
                RevenueByYear = revenueByYear,
                TotalCustomers = db.KhachHangs.Count(),
                MonthlyCustomers = monthlyCust,
                TotalOrders = db.DonHangs.Count(),
                MonthlyOrders = monthlyOrd,
                CategoryProductCounts = catCounts
            };

            return View(vm);
        }
    }
}
