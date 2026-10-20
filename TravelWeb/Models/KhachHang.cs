using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelWeb.Models
{
    [Table("KHACH_HANG")]
    public class KhachHang
    {
        [Key]
        [Column("ma_nguoi_dung")]
        public int MaNguoiDung { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("ho_ten")]
        public string HoTen { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        [Column("so_dienthoai")]
        public string? SoDienthoai { get; set; }

        [MaxLength(255)]
        [Column("mat_khau_hash")]
        public string? MatKhauHash { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("vai_tro")]
        public string VaiTro { get; set; } = "KHACH_HANG";

        public virtual ICollection<DatLich> DatLichs { get; set; } = new List<DatLich>();
    }
}
