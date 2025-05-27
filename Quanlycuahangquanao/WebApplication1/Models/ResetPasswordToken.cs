namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ResetPasswordToken")]
    public partial class ResetPasswordToken
    {
        [Key]
        public int TokenId { get; set; }

        public int MaTK { get; set; }

        public Guid Token { get; set; }

        public DateTime Expiry { get; set; }

        public virtual TaiKhoan TaiKhoan { get; set; }
    }
}
