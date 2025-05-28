create database QLCHQA;
go
use QLCHQA;
go


-- 1. Bảng Khách hàng 
CREATE TABLE KhachHang (
    MaKH INT IDENTITY(1,1) PRIMARY KEY,
    TenKH NVARCHAR(100),
    SoDienThoai NVARCHAR(20),
    AnhDaiDien NVARCHAR(255),
    Email NVARCHAR(100)
);

-- 2. Bảng Loại tài khoản 
CREATE TABLE LoaiTaiKhoan (
    MaLoaiTK INT IDENTITY(1,1) PRIMARY KEY,
    TenLoaiTK NVARCHAR(50)
);

-- 3. Bảng Tài khoản đăng nhập xog
CREATE TABLE TaiKhoan (
    MaTK INT IDENTITY(1,1) PRIMARY KEY,
    TenDangNhap NVARCHAR(50),
    MatKhau NVARCHAR(50),
    NgayTao DATE,
    SoLanDangNhap INT,
    NgayDangNhapGanNhat DATE,
    TrangThai NVARCHAR(20),
    MaLoaiTK INT,
    MaKH INT,
    FOREIGN KEY (MaLoaiTK) REFERENCES LoaiTaiKhoan(MaLoaiTK),
    FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH)
);

-- 4. Bảng Sản phẩm (quần áo)
CREATE TABLE SanPham (
    MaSP INT IDENTITY(1,1) PRIMARY KEY,
    TenSP NVARCHAR(100),
    SoLuong INT,
    GiamGia FLOAT,
    GiaBan FLOAT,
    HinhAnh NVARCHAR(255),
    MoTa NVARCHAR(255),
    TrangThai NVARCHAR(50),
    LaSanPhamMoi BIT,
    KichThuoc NVARCHAR(20),
    NgayTao DATE
);

-- 5. Bảng Đơn hàng
CREATE TABLE DonHang (
    MaDonHang INT IDENTITY(1,1) PRIMARY KEY,
    NgayDat DATE,
    TrangThai NVARCHAR(50),
    MaKH INT,
    MaGiaoHang INT,
    FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH)
);

-- 6. Bảng Chi tiết đơn hàng
CREATE TABLE ChiTietDonHang (
    MaDonHang INT,
    MaSP INT,
    SoLuong INT,
    PRIMARY KEY (MaDonHang, MaSP),
    FOREIGN KEY (MaDonHang) REFERENCES DonHang(MaDonHang),
    FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
);

-- 7. Bảng Giỏ hàng
CREATE TABLE GioHang (
    MaKH INT,
    MaSP INT,
    SoLuong INT,
    PRIMARY KEY (MaKH, MaSP),
    FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH),
    FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
);

-- 8. Bảng Hình ảnh sản phẩm phụ
CREATE TABLE HinhAnhSP (
    MaHinh INT IDENTITY(1,1) PRIMARY KEY,
    UrlHinh NVARCHAR(255),
    MaSP INT,
    FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
);

-- 9. Bảng Danh mục sản phẩm
CREATE TABLE DanhMuc (
    MaDanhMuc INT IDENTITY(1,1) PRIMARY KEY,
    TenDanhMuc NVARCHAR(50)
);

-- 10. Bảng phân loại sản phẩm theo danh mục
CREATE TABLE SanPham_ThuocDanhMuc (
    MaDanhMuc INT,
    MaSanPham INT,
    PRIMARY KEY (MaDanhMuc, MaSanPham),
    FOREIGN KEY (MaDanhMuc) REFERENCES DanhMuc(MaDanhMuc),
    FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSP)
);

-- 11. Bảng Giao hàng
CREATE TABLE GiaoHang (
    MaGiaoHang INT IDENTITY(1,1) PRIMARY KEY,
    TenNguoiNhan NVARCHAR(100),
    DiaChiNhan NVARCHAR(255),
    SoDienThoai NVARCHAR(20),
    LaDiaChiChinh BIT,
    MaKH INT,
    FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH)
);

-- 12. Bảng Quên mật khẩu
CREATE TABLE ResetPasswordToken (
    TokenId INT IDENTITY(1,1) PRIMARY KEY,
    MaTK INT NOT NULL,
    Token UNIQUEIDENTIFIER NOT NULL,
    Expiry DATETIME NOT NULL,
    FOREIGN KEY (MaTK) REFERENCES TaiKhoan(MaTK)
);

-- 13. Bảng quản lý banner quảng cáo
CREATE TABLE Banner (
    MaBanner      INT           IDENTITY(1,1) PRIMARY KEY,  -- Khóa chính, tự tăng
    TieuDe        NVARCHAR(200) NOT NULL,                   -- Tiêu đề / mô tả ngắn
    DuongDanHinh  NVARCHAR(500) NOT NULL,                   -- URL tới file ảnh banner
    DuongDanLienKet NVARCHAR(500) NULL,                     -- Khi click banner sẽ tới link này
    ThuTu         INT           NOT NULL  DEFAULT 0,        -- Thứ tự hiển thị (sort order)
    HienThi       BIT           NOT NULL  DEFAULT 1,        -- 1 = Hiển thị, 0 = Ẩn
    NgayTao       DATETIME      NOT NULL  DEFAULT GETDATE(),-- Ngày tạo
    MoTa          NVARCHAR(500) NULL                        -- Mô tả thêm (nếu cần)
);
GO





USE QLCHQA;
GO

-- Bước 1: Tắt toàn bộ CHECK CONSTRAINT để tránh lỗi FK
EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';

-- Bước 2: Xóa dữ liệu (Dùng DELETE nếu TRUNCATE lỗi do FK)
EXEC sp_MSforeachtable 'DELETE FROM ?';

-- Bước 3: Bật lại CHECK CONSTRAINT
EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';
