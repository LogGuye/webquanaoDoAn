namespace WebApplication1.Models
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GiaoHang")]
    public partial class GiaoHang
    {
        [Key]
        public int? MaGiaoHang { get; set; }

        [StringLength(100)]
        public string TenNguoiNhan { get; set; }

        [StringLength(255)]
        public string DiaChiNhan { get; set; }

        [StringLength(20)]
        public string SoDienThoai { get; set; }

        public bool? LaDiaChiChinh { get; set; }

        public int? MaKH { get; set; }

        public virtual KhachHang KhachHang { get; set; }
    }
}
