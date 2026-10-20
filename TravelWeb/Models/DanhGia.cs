using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelWeb.Models
{
    [Table("DANH_GIA")]
    public class DanhGia
    {
        [Key]
        [Column("ma_danh_gia")]
        public int MaDanhGia { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("ma_dat_lich")]
        public string MaDatLich { get; set; } = string.Empty;

        [Column("ma_khach_hang")]
        public int MaKhachHang { get; set; }

        [Column("ma_dia_diem")]
        public int MaDiaDiem { get; set; }

        [Column("diem_danh_gia")]
        public int DiemDanhGia { get; set; }

        [Column("binh_luan")]
        public string? BinhLuan { get; set; }

        [Column("da_duyet")]
        public int? DaDuyet { get; set; }

        public virtual DatLich? DatLich { get; set; }

        [ForeignKey(nameof(MaKhachHang))]
        public virtual KhachHang? KhachHang { get; set; }

        [ForeignKey(nameof(MaDiaDiem))]
        public virtual DiaDiem? DiaDiem { get; set; }
    }
}
