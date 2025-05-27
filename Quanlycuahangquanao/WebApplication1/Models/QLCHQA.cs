using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Models
{
    public partial class QLCHQA : DbContext
    {
        public QLCHQA()
            : base("name=QLCHQA")
        {
        }

        public virtual DbSet<Banner> Banners { get; set; }
        public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public virtual DbSet<DanhMuc> DanhMucs { get; set; }
        public virtual DbSet<DonHang> DonHangs { get; set; }
        public virtual DbSet<GiaoHang> GiaoHangs { get; set; }
        public virtual DbSet<GioHang> GioHangs { get; set; }
        public virtual DbSet<HinhAnhSP> HinhAnhSPs { get; set; }
        public virtual DbSet<KhachHang> KhachHangs { get; set; }
        public virtual DbSet<LoaiTaiKhoan> LoaiTaiKhoans { get; set; }
        public virtual DbSet<ResetPasswordToken> ResetPasswordTokens { get; set; }
        public virtual DbSet<SanPham> SanPhams { get; set; }
        public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // 1) Mapping many-to-many DanhMuc <-> SanPham qua bảng SanPham_ThuocDanhMuc
            modelBuilder.Entity<DanhMuc>()
                .HasMany(d => d.SanPhams)
                .WithMany(s => s.DanhMucs)
                .Map(m =>
                {
                    m.ToTable("SanPham_ThuocDanhMuc");
                    m.MapLeftKey("MaDanhMuc");
                    m.MapRightKey("MaSanPham");
                });

            // 2) Các mapping khác giữ nguyên:
            modelBuilder.Entity<DonHang>()
                .HasMany(e => e.ChiTietDonHangs)
                .WithRequired(e => e.DonHang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<KhachHang>()
                .HasMany(e => e.GioHangs)
                .WithRequired(e => e.KhachHang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SanPham>()
                .HasMany(e => e.ChiTietDonHangs)
                .WithRequired(e => e.SanPham)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SanPham>()
                .HasMany(e => e.GioHangs)
                .WithRequired(e => e.SanPham)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TaiKhoan>()
                .HasMany(e => e.ResetPasswordTokens)
                .WithRequired(e => e.TaiKhoan)
                .WillCascadeOnDelete(false);
        }
    }
}