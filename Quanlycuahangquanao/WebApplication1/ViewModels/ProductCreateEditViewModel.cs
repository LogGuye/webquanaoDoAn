using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace WebApplication1.ViewModels
{
    public class ProductCreateEditViewModel
    {
        public int MaSP { get; set; }

        [Required]
        [Display(Name = "Tên sản phẩm")]
        public string TenSP { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Số lượng")]
        public int SoLuong { get; set; }

        [Range(0, 100)]
        [Display(Name = "Giảm giá (%)")]
        public double GiamGia { get; set; }

        [Required]
        [Display(Name = "Giá bán")]
        public double GiaBan { get; set; }

        [Display(Name = "Mô tả")]
        public string MoTa { get; set; }

        [Display(Name = "Kích thước")]
        public string KichThuoc { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; }

        [Display(Name = "Sản phẩm mới")]
        public bool? LaSanPhamMoi { get; set; }

        [Display(Name = "Ảnh sản phẩm")]
        public HttpPostedFileBase ImageFile { get; set; }

        // Để giữ lại url cũ khi edit
        public string HinhAnhHienTai { get; set; }

        // Danh sách danh mục đã chọn
        [Display(Name = "Danh mục")]
        public int[] SelectedCategoryIds { get; set; }

        // Dropdown để bind vào view
        public List<SelectListItem> AllCategories { get; set; }
    }
}