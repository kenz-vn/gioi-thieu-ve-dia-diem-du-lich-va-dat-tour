using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelWeb.Models
{
    [Table("NHA_CUNG_CAP")]
    public class NhaCungCap
    {
        [Key]
        [Column("ma_nha_cung_cap")]
        public int MaNhaCungCap { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên nhà cung cấp")]
        [MaxLength(200)]
        [Column("ten_nha_cung_cap")]
        [Display(Name = "Tên nhà cung cấp")]
        public string TenNhaCungCap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn loại dịch vụ")]
        [MaxLength(100)]
        [Column("loai_dich_vu")]
        [Display(Name = "Loại dịch vụ cung cấp")]
        public string LoaiDichVu { get; set; } = "Khách sạn"; // Khách sạn, Xe du lịch, Nhà hàng, Hướng dẫn viên, Vé tham quan

        [MaxLength(100)]
        [Column("nguoi_lien_he")]
        [Display(Name = "Người đại diện liên hệ")]
        public string? NguoiLienHe { get; set; }

        [MaxLength(20)]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Column("so_dien_thoai")]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [MaxLength(100)]
        [Column("email")]
        [Display(Name = "Email đối tác")]
        public string? Email { get; set; }

        [MaxLength(255)]
        [Column("dia_chi")]
        [Display(Name = "Địa chỉ trụ sở / văn phòng")]
        public string? DiaChi { get; set; }

        [MaxLength(30)]
        [Column("trang_thai")]
        [Display(Name = "Trạng thái hợp tác")]
        public string TrangThai { get; set; } = "DANG_HOP_TAC"; // DANG_HOP_TAC, TAM_DUNG

        [Column("ghi_chu")]
        [Display(Name = "Ghi chú hợp đồng / hợp tác")]
        public string? GhiChu { get; set; }

        [Column("ngay_tao")]
        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}
