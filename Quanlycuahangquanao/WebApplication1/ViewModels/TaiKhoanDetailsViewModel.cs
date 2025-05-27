using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.ViewModels
{
    public class TaiKhoanDetailsViewModel
    {
        public int MaTK { get; set; }
        public string TenDangNhap { get; set; }
        public string Email { get; set; }
        public string MatKhau { get; set; }
        public string TenLoaiTK { get; set; }
        public System.DateTime? NgayTao { get; set; }
        public string TrangThai { get; set; }
    }

}