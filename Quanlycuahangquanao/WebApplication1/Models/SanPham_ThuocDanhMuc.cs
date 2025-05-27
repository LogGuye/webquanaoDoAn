using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models;

namespace WebApplication1.Models
{
    public class SanPham_ThuocDanhMuc
    {
        [Key, Column(Order = 0)]
        public int MaDanhMuc { get; set; }

        [Key, Column(Order = 1)]
        public int MaSanPham { get; set; }

        // Navigation tới bảng DanhMuc
        [ForeignKey(nameof(MaDanhMuc))]
        public virtual DanhMuc DanhMuc { get; set; }

        // Navigation tới bảng SanPham
        [ForeignKey(nameof(MaSanPham))]
        public virtual SanPham SanPham { get; set; }
    }
}
