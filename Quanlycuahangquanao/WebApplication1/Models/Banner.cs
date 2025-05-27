namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Banner")]
    public partial class Banner
    {
        [Key]
        public int MaBanner { get; set; }

        [StringLength(200)]
        public string TieuDe { get; set; }

        [Required]
        [StringLength(500)]
        public string DuongDanHinh { get; set; }

        [StringLength(500)]
        public string DuongDanLienKet { get; set; }

        public int ThuTu { get; set; }

        public bool HienThi { get; set; }

        public DateTime NgayTao { get; set; }

        [StringLength(500)]
        public string MoTa { get; set; }
    }
}
