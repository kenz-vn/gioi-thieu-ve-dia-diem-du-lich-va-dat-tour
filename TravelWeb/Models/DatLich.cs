using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelWeb.Models
{
    [Table("DAT_LICH")]
    public class DatLich
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("ma_dat_lich")]
        public string MaDatLich { get; set; } = string.Empty;

        [Column("ma_khach_hang")]
        public int? MaKhachHang { get; set; }

        [Column("ma_dich_vu")]
        public int MaDichVu { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("ma_tra_cuu")]
        public string MaTraCuu { get; set; } = string.Empty;

        [Column("ngay_tham_quan")]
        public DateTime NgayThamQuan { get; set; }

        [Column("so_luong_khach")]
        [Display(Name = "Tổng số khách")]
        public int SoLuongKhach { get; set; }

        [Column("so_luong_nguoi_lon")]
        [Display(Name = "Số người lớn")]
        public int SoLuongNguoiLon { get; set; } = 1;

        [Column("so_luong_tre_em")]
        [Display(Name = "Số trẻ em")]
        public int SoLuongTreEm { get; set; } = 0;

        [Column("ngay_check_in")]
        [Display(Name = "Thời gian Check-in")]
        public DateTime? NgayCheckIn { get; set; }

        [Column("ngay_check_out")]
        [Display(Name = "Thời gian Check-out")]
        public DateTime? NgayCheckOut { get; set; }

        [MaxLength(255)]
        [Column("ten_khach_san")]
        [Display(Name = "Khách sạn")]
        public string? TenKhachSan { get; set; }

        [MaxLength(100)]
        [Column("loai_phong")]
        [Display(Name = "Loại phòng / Giường")]
        public string? LoaiPhong { get; set; }

        [Column("so_luong_phong")]
        [Display(Name = "Số phòng đặt")]
        public int SoLuongPhong { get; set; } = 1;

        [Column("gia_nguoi_lon", TypeName = "decimal(18,2)")]
        [Display(Name = "Giá vé người lớn")]
        public decimal GiaNguoiLon { get; set; }

        [Column("gia_tre_em", TypeName = "decimal(18,2)")]
        [Display(Name = "Giá vé trẻ em")]
        public decimal GiaTreEm { get; set; }

        [Column("tong_tien", TypeName = "decimal(18,2)")]
        [Display(Name = "Tổng tiền tour")]
        public decimal TongTien { get; set; }

        [Column("so_tien_coc", TypeName = "decimal(18,2)")]
        [Display(Name = "Số tiền cọc")]
        public decimal SoTienCoc { get; set; }

        [MaxLength(20)]
        [Column("trang_thai_dat")]
        public string? TrangThaiDat { get; set; } // CHO_XAC_NHAN, DA_XAC_NHAN, TU_CHOI, HOAN_THANH, DA_HUY

        [MaxLength(50)]
        [Column("ma_voucher")]
        public string? MaVoucher { get; set; }

        [MaxLength(50)]
        [Column("hinh_thuc")]
        public string? HinhThuc { get; set; }

        [Column("thoi_gian_tao")]
        public DateTime? ThoiGianTao { get; set; }

        [MaxLength(100)]
        [Column("ho_ten")]
        public string? HoTen { get; set; }

        [MaxLength(100)]
        [Column("email")]
        public string? Email { get; set; }

        [MaxLength(20)]
        [Column("so_dienthoai")]
        public string? SoDienthoai { get; set; }

        // --- CÁC TRƯỜNG PHỤC VỤ DL-19 (KIÊN) ---
        [MaxLength(500)]
        [Column("ly_do_tu_choi")]
        [Display(Name = "Lý do từ chối")]
        public string? LyDoTuChoi { get; set; }

        [MaxLength(100)]
        [Column("nguoi_xu_ly")]
        [Display(Name = "Nhân viên xử lý")]
        public string? NguoiXuLy { get; set; }

        [Column("thoi_gian_xu_ly")]
        [Display(Name = "Thời gian xử lý")]
        public DateTime? ThoiGianXuLy { get; set; }

        [MaxLength(500)]
        [Column("ghi_chu_xu_ly")]
        [Display(Name = "Ghi chú xử lý nội bộ")]
        public string? GhiChuXuLy { get; set; }

        [ForeignKey(nameof(MaDichVu))]
        public virtual DichVu? DichVu { get; set; }

        [ForeignKey(nameof(MaKhachHang))]
        public virtual KhachHang? KhachHang { get; set; }

        public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
        public virtual ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();
    }
}
