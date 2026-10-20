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
        public int SoLuongKhach { get; set; }

        [Column("so_tien_coc", TypeName = "decimal(18,2)")]
        public decimal SoTienCoc { get; set; }

        [MaxLength(20)]
        [Column("trang_thai_dat")]
        public string? TrangThaiDat { get; set; }

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

        [ForeignKey(nameof(MaDichVu))]
        public virtual DichVu? DichVu { get; set; }

        [ForeignKey(nameof(MaKhachHang))]
        public virtual KhachHang? KhachHang { get; set; }

        public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
        public virtual ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();
    }
}
