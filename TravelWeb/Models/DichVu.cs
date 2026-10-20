using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelWeb.Models
{
    [Table("DICH_VU")]
    public class DichVu
    {
        [Key]
        [Column("ma_dich_vu")]
        public int MaDichVu { get; set; }

        [Column("ma_dia_diem")]
        public int MaDiaDiem { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("ten_dich_vu")]
        public string TenDichVu { get; set; } = string.Empty;

        [Column("gia_tien", TypeName = "decimal(18,2)")]
        public decimal GiaTien { get; set; }

        [Column("ty_le_coc", TypeName = "decimal(5,2)")]
        public decimal TyLeCoc { get; set; } = 30;

        [MaxLength(255)]
        [Column("hinh_anh_phong")]
        public string? HinhAnhPhong { get; set; }

        [ForeignKey(nameof(MaDiaDiem))]
        public virtual DiaDiem? DiaDiem { get; set; }

        public virtual ICollection<DatLich> DatLichs { get; set; } = new List<DatLich>();
    }
}
