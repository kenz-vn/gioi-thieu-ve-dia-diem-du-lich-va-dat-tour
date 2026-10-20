using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelWeb.Models
{
    [Table("DANH_MUC")]
    public class DanhMuc
    {
        [Key]
        [Column("ma_danh_muc")]
        public int MaDanhMuc { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("ten_danh_muc")]
        public string TenDanhMuc { get; set; } = string.Empty;

        public virtual ICollection<DiaDiem> DiaDiems { get; set; } = new List<DiaDiem>();
    }
}
