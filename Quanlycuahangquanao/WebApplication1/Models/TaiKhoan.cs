namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TaiKhoan")]
    public partial class TaiKhoan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TaiKhoan()
        {
            ResetPasswordTokens = new HashSet<ResetPasswordToken>();
        }

        [Key]
        public int MaTK { get; set; }

        [StringLength(50)]
        public string TenDangNhap { get; set; }

        [StringLength(50)]
        public string MatKhau { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NgayTao { get; set; }

        public int? SoLanDangNhap { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NgayDangNhapGanNhat { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; }

        public int? MaLoaiTK { get; set; }

        public int? MaKH { get; set; }

        public virtual KhachHang KhachHang { get; set; }

        public virtual LoaiTaiKhoan LoaiTaiKhoan { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ResetPasswordToken> ResetPasswordTokens { get; set; }
    }
}
