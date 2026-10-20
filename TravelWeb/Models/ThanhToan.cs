using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelWeb.Models
{
    [Table("THANH_TOAN")]
    public class ThanhToan
    {
        [Key]
        [Column("ma_thanh_toan")]
        public int MaThanhToan { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("ma_dat_lich")]
        public string MaDatLich { get; set; } = string.Empty;

        [Column("so_tien", TypeName = "decimal(18,2)")]
        public decimal SoTien { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("phuong_thuc_tt")]
        public string PhuongThucTt { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("ma_giao_dich")]
        public string? MaGiaoDich { get; set; }

        [MaxLength(20)]
        [Column("trang_thai_tt")]
        public string? TrangThaiTt { get; set; }

        public virtual DatLich? DatLich { get; set; }
    }
}
