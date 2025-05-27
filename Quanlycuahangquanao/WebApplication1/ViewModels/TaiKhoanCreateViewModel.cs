using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace WebApplication1.ViewModels
{
    public class TaiKhoanCreateViewModel
    {
        [Required]
        [Display(Name = "Email (Tài khoản)")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Mật khẩu")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }

        [Required]
        [Display(Name = "Loại tài khoản")]
        public int? MaLoaiTK { get; set; }

        // For dropdown list
        public IEnumerable<SelectListItem> ListLoaiTK { get; set; }
    }
}
