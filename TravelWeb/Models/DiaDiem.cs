using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelWeb.Models
{
    [Table("DIA_DIEM")]
    public class DiaDiem
    {
        [Key]
        [Column("ma_dia_diem")]
        public int MaDiaDiem { get; set; }

        [Column("ma_chu_so_huu")]
        public int? MaChuSoHuu { get; set; }

        [Column("ma_danh_muc")]
        public int MaDanhMuc { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("ten_dia_diem")]
        public string TenDiaDiem { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [Column("dia_chi")]
        public string DiaChi { get; set; } = string.Empty;

        [MaxLength(20)]
        [Column("hotline")]
        public string? Hotline { get; set; }

        [MaxLength(20)]
        [Column("trang_thai")]
        public string? TrangThai { get; set; }

        [MaxLength(255)]
        [Column("hinh_anh")]
        public string? HinhAnh { get; set; }

        [Column("van_hoa")]
        public string? VanHoa { get; set; }

        [Column("do_an")]
        public string? DoAn { get; set; }

        [Column("noi_can_toi")]
        public string? NoiCanToi { get; set; }

        [ForeignKey(nameof(MaDanhMuc))]
        public virtual DanhMuc? DanhMuc { get; set; }

        [ForeignKey(nameof(MaChuSoHuu))]
        public virtual KhachHang? ChuSoHuu { get; set; }

        public virtual ICollection<DichVu> DichVus { get; set; } = new List<DichVu>();
    }
}
