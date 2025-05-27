using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.ViewModels
{
    public class TaiKhoanIndexViewModel
    {
        public int MaTK { get; set; }
        public string TenDangNhap { get; set; }  // Email
        public string Email { get; set; }
        public string MatKhau { get; set; }
        public string TenLoaiTK { get; set; }    // Tên loại tài khoản (Admin/Nhân viên/Khách hàng)
    }
}