using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.ViewModels
{
    public class ProductIndexViewModel
    {
        public int MaSP { get; set; }
        public string HinhAnh { get; set; }
        public string TenSP { get; set; }
        public int SoLuong { get; set; }
        public double GiamGia { get; set; }
        public double GiaBan { get; set; }
        public string TrangThai { get; set; }
        public bool? LaSanPhamMoi { get; set; }
    }
}
