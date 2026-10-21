using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Models;

namespace TravelWeb.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. HIỂN THỊ TRANG ĐẶT TOUR (GET) - UC-01
        [HttpGet]
        public async Task<IActionResult> DatTour(int id)
        {
            // Điểm rẽ nhánh 1: Kiểm tra đăng nhập (E1)
            string? currentUser = HttpContext.Session.GetString("CURRENT_USER");
            if (string.IsNullOrEmpty(currentUser))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập trước khi tiến hành đặt tour.";
                return RedirectToAction("DangNhap", "TaiKhoan", new { returnUrl = $"/Booking/DatTour/{id}" });
            }

            var dichVu = await _context.DichVus
                .Include(d => d.DiaDiem)
                .FirstOrDefaultAsync(d => d.MaDichVu == id);

            if (dichVu == null)
            {
                return NotFound();
            }

            // Điền sẵn thông tin khách hàng đã đăng nhập
            ViewBag.HoTen = currentUser;
            ViewBag.Email = HttpContext.Session.GetString("USER_EMAIL");
            ViewBag.SoDienthoai = HttpContext.Session.GetString("USER_PHONE");

            return View(dichVu);
        }

        // 2. XỬ LÝ LƯU TRỮ ĐƠN ĐẶT & TẠM GIỮ CHỖ (POST) - UC-01
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanDatTour(int maDichVu, int soLuongKhach, DateTime ngayThamQuan, string hoTen, string email, string soDienthoai)
        {
            // Kiểm tra đăng nhập
            int? sessionKhId = HttpContext.Session.GetInt32("USER_ID");
            if (!sessionKhId.HasValue)
            {
                TempData["ErrorMessage"] = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("DangNhap", "TaiKhoan", new { returnUrl = $"/Booking/DatTour/{maDichVu}" });
            }

            var dichVu = await _context.DichVus.FindAsync(maDichVu);
            if (dichVu == null) return NotFound();

            // Điểm rẽ nhánh 2: Kiểm tra ngày đi (E2)
            if (ngayThamQuan.Date <= DateTime.Today)
            {
                TempData["ErrorMessage"] = "Ngày đi không hợp lệ. Vui lòng chọn ngày khởi hành từ ngày mai trở đi.";
                return RedirectToAction("DatTour", new { id = maDichVu });
            }

            // Điểm rẽ nhánh 3: Kiểm tra số lượng người (E3)
            if (soLuongKhach <= 0)
            {
                TempData["ErrorMessage"] = "Số lượng người không hợp lệ. Vui lòng nhập tối thiểu 1 người.";
                return RedirectToAction("DatTour", new { id = maDichVu });
            }

            // Điểm rẽ nhánh 4: Kiểm tra sức chứa (E4)
            if (soLuongKhach > dichVu.SoChoConLai)
            {
                TempData["ErrorMessage"] = $"Tour không đủ chỗ! Hiện tại chỉ còn {dichVu.SoChoConLai} chỗ khả dụng.";
                return RedirectToAction("DatTour", new { id = maDichVu });
            }

            // Tính toán tổng tiền & tiền cọc
            decimal giaApDung = (dichVu.GiaKhuyenMai.HasValue && dichVu.GiaKhuyenMai.Value > 0) ? dichVu.GiaKhuyenMai.Value : dichVu.GiaTien;
            decimal tongTien = giaApDung * soLuongKhach;
            decimal soTienCoc = tongTien * (dichVu.TyLeCoc / 100);

            // Sinh mã tra cứu công khai và mã đặt lịch duy nhất
            string maTraCuu = "TOUR-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(1000, 9999);
            string maDatLich = "BK" + DateTime.Now.Ticks.ToString().Substring(10);

            // Tạo đối tượng lưu vào bảng DAT_LICH
            var datLich = new DatLich
            {
                MaDatLich = maDatLich,
                MaDichVu = maDichVu,
                MaTraCuu = maTraCuu,
                NgayThamQuan = ngayThamQuan,
                SoLuongKhach = soLuongKhach,
                SoTienCoc = soTienCoc,
                TrangThaiDat = "CHO_XAC_NHAN",
                ThoiGianTao = DateTime.Now,
                HoTen = hoTen,
                Email = email,
                SoDienthoai = soDienthoai,
                MaKhachHang = sessionKhId.Value
            };

            // Tạm giữ số chỗ (Tránh vượt quá sức chứa)
            dichVu.SoChoConLai -= soLuongKhach;
            if (dichVu.SoChoConLai == 0)
            {
                dichVu.TrangThaiTour = "HET_CHO";
            }

            _context.DatLichs.Add(datLich);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đặt tour thành công! Mã tra cứu của bạn là: {maTraCuu}";
            return RedirectToAction("LichSuDatTour", new { maTraCuu = maTraCuu });
        }

        // 3. XEM LỊCH SỬ ĐẶT TOUR (GET) - UC-03
        [HttpGet]
        public async Task<IActionResult> LichSuDatTour(string? maTraCuu, int? maKhachHang)
        {
            var query = _context.DatLichs
                .Include(d => d.DichVu)
                .ThenInclude(dv => dv!.DiaDiem)
                .AsQueryable();

            if (!string.IsNullOrEmpty(maTraCuu))
            {
                query = query.Where(d => d.MaTraCuu == maTraCuu.Trim());
            }
            else
            {
                int? sessionKhId = HttpContext.Session.GetInt32("USER_ID");
                string? role = HttpContext.Session.GetString("CURRENT_ROLE");

                if (maKhachHang.HasValue && (role == "ADMIN" || role == "NHAN_VIEN"))
                {
                    query = query.Where(d => d.MaKhachHang == maKhachHang.Value);
                }
                else if (sessionKhId.HasValue)
                {
                    query = query.Where(d => d.MaKhachHang == sessionKhId.Value);
                }
                else
                {
                    // Nếu chưa đăng nhập và không tra cứu mã cụ thể, hướng dẫn đăng nhập
                    TempData["ErrorMessage"] = "Vui lòng đăng nhập hoặc nhập mã tra cứu để xem lịch sử đặt tour.";
                    return RedirectToAction("DangNhap", "TaiKhoan", new { returnUrl = "/Booking/LichSuDatTour" });
                }
            }

            var danhSachDon = await query.OrderByDescending(d => d.ThoiGianTao).ToListAsync();
            ViewBag.MaTraCuu = maTraCuu;
            return View(danhSachDon);
        }

        // 4. XEM CHI TIẾT ĐƠN ĐẶT TOUR (GET) - UC-03
        [HttpGet]
        public async Task<IActionResult> ChiTietDon(int id)
        {
            var don = await _context.DatLichs
                .Include(d => d.DichVu)
                .ThenInclude(dv => dv!.DiaDiem)
                .Include(d => d.ThanhToans)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (don == null) return NotFound();

            // Điểm rẽ nhánh 2: Kiểm tra quyền sở hữu (E1)
            int? sessionKhId = HttpContext.Session.GetInt32("USER_ID");
            string? role = HttpContext.Session.GetString("CURRENT_ROLE");

            if (role != "ADMIN" && role != "NHAN_VIEN" && don.MaKhachHang != sessionKhId)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền xem đơn đặt tour này (E1).";
                return RedirectToAction(nameof(LichSuDatTour));
            }

            return View(don);
        }

        // 5. HỦY ĐƠN ĐẶT TOUR CÓ ĐIỀU KIỆN (POST) - UC-04
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDon(int id, string? lyDoHuy)
        {
            int? sessionKhId = HttpContext.Session.GetInt32("USER_ID");
            string? role = HttpContext.Session.GetString("CURRENT_ROLE");

            var don = await _context.DatLichs
                .Include(d => d.DichVu)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (don == null) return NotFound();

            // Điểm rẽ nhánh 1: Kiểm tra quyền sở hữu (E1)
            if (role != "ADMIN" && role != "NHAN_VIEN" && don.MaKhachHang != sessionKhId)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền thao tác hủy đơn đặt tour này (E1).";
                return RedirectToAction(nameof(LichSuDatTour));
            }

            // Điểm rẽ nhánh 2: Kiểm tra trạng thái đơn (E2, E3)
            if (don.TrangThaiDat == "DA_HUY")
            {
                TempData["ErrorMessage"] = "Đơn đặt tour đã được hủy trước đó (E2).";
                return RedirectToAction(nameof(LichSuDatTour));
            }

            if (don.TrangThaiDat == "HOAN_THANH" || don.TrangThaiDat == "TU_CHOI")
            {
                TempData["ErrorMessage"] = "Không thể hủy đơn đặt tour ở trạng thái này (E3).";
                return RedirectToAction(nameof(LichSuDatTour));
            }

            // Điểm rẽ nhánh 3: Kiểm tra thời hạn hủy (E4) - Tối thiểu trước 2 ngày so với ngày khởi hành
            if (don.NgayThamQuan.Date <= DateTime.Today.AddDays(2))
            {
                TempData["ErrorMessage"] = "Đã quá thời hạn hủy đơn đặt tour (E4). Quy định chỉ cho phép hủy trước ngày khởi hành tối thiểu 2 ngày.";
                return RedirectToAction(nameof(LichSuDatTour));
            }

            // Thực hiện giao dịch hủy đơn
            don.TrangThaiDat = "DA_HUY";
            don.LyDoTuChoi = string.IsNullOrWhiteSpace(lyDoHuy) ? "Khách hàng tự hủy qua website" : $"Khách hủy: {lyDoHuy.Trim()}";
            don.ThoiGianXuLy = DateTime.Now;
            don.NguoiXuLy = HttpContext.Session.GetString("CURRENT_USER") ?? "Khách hàng";

            // Giải phóng số chỗ đã giữ cho tour
            if (don.DichVu != null)
            {
                don.DichVu.SoChoConLai = Math.Min(don.DichVu.SoChoToiDa, don.DichVu.SoChoConLai + don.SoLuongKhach);
                if (don.DichVu.TrangThaiTour == "HET_CHO" && don.DichVu.SoChoConLai > 0)
                {
                    don.DichVu.TrangThaiTour = "DANG_BAN";
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Hủy đơn đặt tour ({don.MaTraCuu}) thành công! Số chỗ ({don.SoLuongKhach} khách) đã được giải phóng.";
            return RedirectToAction(nameof(LichSuDatTour));
        }
    }
}
