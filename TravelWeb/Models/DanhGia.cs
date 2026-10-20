using System;
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

        [Range(1, 5, ErrorMessage = "Điểm đánh giá từ 1 đến 5 sao")]
        [Column("diem_danh_gia")]
        [Display(Name = "Điểm đánh giá")]
        public int DiemDanhGia { get; set; }

        [Column("binh_luan")]
        [Display(Name = "Nội dung nhận xét của khách")]
        public string? BinhLuan { get; set; }

        [Column("da_duyet")]
        [Display(Name = "Trạng thái duyệt")]
        public int? DaDuyet { get; set; } = 0; // 0: Chờ duyệt, 1: Đã duyệt, -1: Từ chối / Ẩn

        // --- CÁC TRƯỜNG MỞ RỘNG PHỤC VỤ DL-22 (KIÊN) ---
        [Column("ngay_danh_gia")]
        [Display(Name = "Ngày gửi đánh giá")]
        public DateTime? NgayDanhGia { get; set; } = DateTime.Now;

        [MaxLength(1000)]
        [Column("phan_hoi_cua_nhan_vien")]
        [Display(Name = "Phản hồi của nhân viên")]
        public string? PhanHoiCuaNhanVien { get; set; }

        [MaxLength(500)]
        [Column("ghi_chu_kiem_duyet")]
        [Display(Name = "Ghi chú kiểm duyệt")]
        public string? GhiChuKiemDuyet { get; set; }

        public virtual DatLich? DatLich { get; set; }

        [ForeignKey(nameof(MaKhachHang))]
        public virtual KhachHang? KhachHang { get; set; }

        [ForeignKey(nameof(MaDiaDiem))]
        public virtual DiaDiem? DiaDiem { get; set; }
    }
}
