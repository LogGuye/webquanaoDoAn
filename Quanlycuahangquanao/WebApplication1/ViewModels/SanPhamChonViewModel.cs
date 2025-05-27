using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class SanPhamChonViewModel
{
    public int MaSP { get; set; }
    public string TenSP { get; set; }
    public decimal GiaBan { get; set; }
    public double GiamGia { get; set; }
    public int SoLuongTon { get; set; }
    public int SoLuong { get; set; } = 1;
    public bool IsChecked { get; set; }

    public List<SanPhamChonViewModel> SanPhamChons { get; set; } = new List<SanPhamChonViewModel>();

    public decimal ThanhTien
    {
        get
        {
            var giaSauGiam = GiaBan * (decimal)(1 - GiamGia / 100);
            return giaSauGiam * SoLuong;
        }
    }
}