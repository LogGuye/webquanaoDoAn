using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.ViewModels
{
    public class DonHangIndexViewModel
    {
        public int MaDonHang { get; set; }
        public DateTime NgayDat { get; set; }
        public string TenKH { get; set; }
        public string DiaChiGiao { get; set; }
        public decimal TongTien { get; set; }
        public decimal TongTienSauGiam { get; set; }
        public string TrangThai { get; set; }
    }

}