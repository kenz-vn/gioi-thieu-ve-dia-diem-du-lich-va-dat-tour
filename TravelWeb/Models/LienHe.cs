using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelWeb.Models
{
    [Table("LIEN_HE")]
    public class LienHe
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [MaxLength(100)]
        [Column("ho_ten")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [MaxLength(100)]
        [Column("email")]
        [Display(Name = "Địa chỉ Email")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Column("so_dien_thoai")]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề liên hệ")]
        [MaxLength(200)]
        [Column("tieu_de")]
        [Display(Name = "Tiêu đề yêu cầu")]
        public string TieuDe { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập nội dung liên hệ")]
        [Column("noi_dung")]
        [Display(Name = "Nội dung liên hệ")]
        public string NoiDung { get; set; } = string.Empty;

        [Column("ngay_gui")]
        [Display(Name = "Ngày gửi")]
        public DateTime NgayGui { get; set; } = DateTime.Now;

        [MaxLength(30)]
        [Column("trang_thai")]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "CHUA_XU_LY"; // CHUA_XU_LY, DANG_XU_LY, DA_XU_LY

        [Column("phan_hoi")]
        [Display(Name = "Nội dung phản hồi khách hàng")]
        public string? PhanHoi { get; set; }

        [MaxLength(100)]
        [Column("nguoi_xu_ly")]
        [Display(Name = "Nhân viên xử lý")]
        public string? NguoiXuLy { get; set; }

        [Column("ngay_xu_ly")]
        [Display(Name = "Thời gian xử lý")]
        public DateTime? NgayXuLy { get; set; }
    }
}
