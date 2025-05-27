using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.ViewModels
{
    public class DonHangCreateViewModel
    {
        [Display(Name = "Mã giao hàng")]
        public string MaGiaoHang { get; set; }

        [Display(Name = "Mã đơn hàng")]
        public string MaDonHang { get; set; }

        [Display(Name = "Mã giảm giá")]
        public string MaGiamGia { get; set; }

        [Display(Name = "Tên khách hàng")]
        [Required(ErrorMessage = "Tên khách hàng bắt buộc")]
        public string TenKH { get; set; }

        [Display(Name = "Địa chỉ nhận")]
        [Required(ErrorMessage = "Địa chỉ giao bắt buộc")]
        public string DiaChiNhan { get; set; }

        [Display(Name = "Ngày đặt")]
        public DateTime NgayDat { get; set; } = DateTime.Now;

        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }
        public List<SanPhamChonViewModel> SanPhamChons { get; set; } = new List<SanPhamChonViewModel>();
        public string SearchSanPham { get; set; }
        public List<SanPhamChonViewModel> SanPhams { get; set; } = new List<SanPhamChonViewModel>();// Sản phẩm hiển thị tìm kiếm
        public decimal TongTien => SanPhamChons?.Sum(x => x.ThanhTien) ?? 0;
    }

}