using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class KhachHangEditViewModel
    {
        public int MaKH { get; set; }

        [Required(ErrorMessage = "Tên khách hàng không được để trống")]
        public string TenKH { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "SĐT phải là 10 số bắt đầu bằng 0")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        public string AnhDaiDien { get; set; }
    }
}
