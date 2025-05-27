namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HinhAnhSP")]
    public partial class HinhAnhSP
    {
        [Key]
        public int MaHinh { get; set; }

        [StringLength(255)]
        public string UrlHinh { get; set; }

        public int? MaSP { get; set; }

        public virtual SanPham SanPham { get; set; }
    }
}
