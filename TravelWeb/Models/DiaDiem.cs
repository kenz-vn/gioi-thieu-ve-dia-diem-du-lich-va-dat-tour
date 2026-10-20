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
        [Display(Name = "Mã địa điểm")]
        public int MaDiaDiem { get; set; }

        [Column("ma_chu_so_huu")]
        [Display(Name = "Chủ sở hữu")]
        public int? MaChuSoHuu { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục địa điểm")]
        [Column("ma_danh_muc")]
        [Display(Name = "Danh mục")]
        public int MaDanhMuc { get; set; }

        [Required(ErrorMessage = "Tên địa điểm không được để trống")]
        [MaxLength(255)]
        [Column("ten_dia_diem")]
        [Display(Name = "Tên địa điểm du lịch")]
        public string TenDiaDiem { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [MaxLength(255)]
        [Column("dia_chi")]
        [Display(Name = "Địa chỉ chi tiết")]
        public string DiaChi { get; set; } = string.Empty;

        [MaxLength(20)]
        [Column("hotline")]
        [Display(Name = "Hotline / Số điện thoại")]
        public string? Hotline { get; set; }

        [MaxLength(20)]
        [Column("trang_thai")]
        [Display(Name = "Trạng thái hoạt động")]
        public string? TrangThai { get; set; } = "HOAT_DONG"; // HOAT_DONG, DONG_CUA, BAO_TRI

        [MaxLength(255)]
        [Column("hinh_anh")]
        [Display(Name = "Hình ảnh đại diện")]
        public string? HinhAnh { get; set; }

        [Column("van_hoa")]
        [Display(Name = "Đặc trưng văn hóa")]
        public string? VanHoa { get; set; }

        [Column("do_an")]
        [Display(Name = "Ẩm thực đặc sản")]
        public string? DoAn { get; set; }

        [Column("noi_can_toi")]
        [Display(Name = "Các điểm vui chơi / Nơi cần tới")]
        public string? NoiCanToi { get; set; }

        [ForeignKey(nameof(MaDanhMuc))]
        public virtual DanhMuc? DanhMuc { get; set; }

        [ForeignKey(nameof(MaChuSoHuu))]
        public virtual KhachHang? ChuSoHuu { get; set; }

        public virtual ICollection<DichVu> DichVus { get; set; } = new List<DichVu>();
    }
}
