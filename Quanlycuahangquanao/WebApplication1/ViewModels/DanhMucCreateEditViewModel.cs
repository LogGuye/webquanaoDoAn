using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.ViewModels
{
    public class DanhMucCreateEditViewModel
    {
        public int? MaDanhMuc { get; set; }
        [Required, StringLength(50)]
        public string TenDanhMuc { get; set; }
    }
}