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
        [Display(Name = "Địa điểm")]
        public int MaDiaDiem { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên tour / dịch vụ")]
        [MaxLength(255)]
        [Column("ten_dich_vu")]
        [Display(Name = "Tên tour / Dịch vụ")]
        public string TenDichVu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập giá tiền")]
        [Range(0, 1000000000, ErrorMessage = "Giá tiền phải từ 0 trở lên")]
        [Column("gia_tien", TypeName = "decimal(18,2)")]
        [Display(Name = "Giá gốc (VNĐ)")]
        public decimal GiaTien { get; set; }

        [Column("ty_le_coc", TypeName = "decimal(5,2)")]
        [Range(1, 100, ErrorMessage = "Tỷ lệ cọc từ 1% đến 100%")]
        [Display(Name = "Tỷ lệ cọc (%)")]
        public decimal TyLeCoc { get; set; } = 30;

        [MaxLength(255)]
        [Column("hinh_anh_phong")]
        [Display(Name = "Hình ảnh đại diện")]
        public string? HinhAnhPhong { get; set; }

        // --- CÁC TRƯỜNG MỞ RỘNG PHỤC VỤ DL-13 (KIÊN) ---
        [Range(1, 1000, ErrorMessage = "Số chỗ tối đa phải từ 1 trở lên")]
        [Column("so_cho_toi_da")]
        [Display(Name = "Số chỗ tối đa")]
        public int SoChoToiDa { get; set; } = 30;

        [Range(0, 1000, ErrorMessage = "Số chỗ còn lại không được phép âm!")]
        [Column("so_cho_con_lai")]
        [Display(Name = "Số chỗ còn lại")]
        public int SoChoConLai { get; set; } = 30;

        [MaxLength(50)]
        [Column("trang_thai_tour")]
        [Display(Name = "Trạng thái tour")]
        public string TrangThaiTour { get; set; } = "DANG_BAN"; // DANG_BAN, TAM_NGUNG, HET_CHO

        [Column("lich_trinh")]
        [Display(Name = "Lịch trình chi tiết")]
        public string? LichTrinh { get; set; }

        [Range(0, 1000000000, ErrorMessage = "Giá khuyến mãi phải lớn hơn hoặc bằng 0")]
        [Column("gia_khuyen_mai", TypeName = "decimal(18,2)")]
        [Display(Name = "Giá khuyến mãi (VNĐ)")]
        public decimal? GiaKhuyenMai { get; set; }

        [MaxLength(100)]
        [Column("thoi_luong")]
        [Display(Name = "Thời lượng tour")]
        public string? ThoiLuong { get; set; } = "3 ngày 2 đêm";

        [ForeignKey(nameof(MaDiaDiem))]
        public virtual DiaDiem? DiaDiem { get; set; }

        public virtual ICollection<DatLich> DatLichs { get; set; } = new List<DatLich>();
    }
}
