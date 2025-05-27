namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using WebApplication1.Models;

    [Table("SanPham")]
    public partial class SanPham
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SanPham()
        {
            ChiTietDonHangs = new HashSet<ChiTietDonHang>();
            GioHangs = new HashSet<GioHang>();
            HinhAnhSPs = new HashSet<HinhAnhSP>();
            DanhMucs = new HashSet<DanhMuc>();
        }

        [Key]
        public int MaSP { get; set; }

        [StringLength(100)]
        public string TenSP { get; set; }

        public int? SoLuong { get; set; }

        public double? GiamGia { get; set; }

        public double? GiaBan { get; set; }

        [StringLength(255)]
        public string HinhAnh { get; set; }

        [StringLength(500)]
        public string MoTa { get; set; }

        [StringLength(50)]
        public string TrangThai { get; set; }

        public bool? LaSanPhamMoi { get; set; }

        [StringLength(20)]
        public string KichThuoc { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NgayTao { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GioHang> GioHangs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HinhAnhSP> HinhAnhSPs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DanhMuc> DanhMucs { get; set; }
    }
}
