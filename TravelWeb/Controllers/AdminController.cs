using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Filters;
using TravelWeb.Models;

namespace TravelWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Dashboard trung tâm điều hành quản trị
        [PhanQuyen("ADMIN", "NHAN_VIEN")]
        public async Task<IActionResult> Index()
        {
            // Lấy vai trò hiện tại trong Session
            string? role = HttpContext.Session.GetString("CURRENT_ROLE");
            if (string.IsNullOrEmpty(role))
            {
                return RedirectToAction("DangNhap", "TaiKhoan", new { returnUrl = "/Admin" });
            }

            ViewBag.CurrentRole = role;
            ViewBag.CurrentUser = HttpContext.Session.GetString("CURRENT_USER");

            // Thống kê phục vụ Dashboard
            ViewBag.TongDiaDiem = await _context.DiaDiems.CountAsync();
            ViewBag.TongTour = await _context.DichVus.CountAsync();
            ViewBag.TourDangBan = await _context.DichVus.CountAsync(t => t.TrangThaiTour == "DANG_BAN");
            ViewBag.TourTamNgung = await _context.DichVus.CountAsync(t => t.TrangThaiTour == "TAM_NGUNG" || t.TrangThaiTour == "HET_CHO");
            
            // Thống kê Đơn đặt (DL-19)
            ViewBag.TongDonDat = await _context.DatLichs.CountAsync();
            ViewBag.DonChoXacNhan = await _context.DatLichs.CountAsync(d => d.TrangThaiDat == "CHO_XAC_NHAN");
            ViewBag.DonDaXacNhan = await _context.DatLichs.CountAsync(d => d.TrangThaiDat == "DA_XAC_NHAN");
            ViewBag.DonTuChoi = await _context.DatLichs.CountAsync(d => d.TrangThaiDat == "TU_CHOI");
            
            // Thống kê Đánh giá (DL-22)
            ViewBag.DanhGiaChoDuyet = await _context.DanhGias.CountAsync(g => g.DaDuyet == 0);
            ViewBag.DanhGiaDaDuyet = await _context.DanhGias.CountAsync(g => g.DaDuyet == 1);

            // Thống kê Liên hệ (DL-22)
            ViewBag.LienHeChuaXuLy = await _context.LienHes.CountAsync(l => l.TrangThai == "CHUA_XU_LY");

            // Thống kê Nhà cung cấp (DL-22)
            ViewBag.TongNhaCungCap = await _context.NhaCungCaps.CountAsync();

            // Đơn mới nhất
            var recentBookings = await _context.DatLichs
                .Include(d => d.DichVu)
                .OrderByDescending(d => d.ThoiGianTao)
                .Take(5)
                .ToListAsync();

            return View(recentBookings);
        }

        // Chuyển đổi vai trò linh hoạt phục vụ chấm bài / demo phân quyền
        [HttpPost]
        public IActionResult SwitchRole(string role, string returnUrl = "")
        {
            if (role == "ADMIN")
            {
                HttpContext.Session.SetString("CURRENT_ROLE", "ADMIN");
                HttpContext.Session.SetString("CURRENT_USER", "Nguyễn Quản Trị (Admin)");
            }
            else if (role == "NHAN_VIEN")
            {
                HttpContext.Session.SetString("CURRENT_ROLE", "NHAN_VIEN");
                HttpContext.Session.SetString("CURRENT_USER", "Bùi Trung Kiên (Nhân viên)");
            }
            else
            {
                HttpContext.Session.SetString("CURRENT_ROLE", "KHACH_HANG");
                HttpContext.Session.SetString("CURRENT_USER", "Trần Minh Hoàng (Khách hàng)");
            }

            TempData["SuccessMessage"] = $"Đã chuyển vai trò sang: {HttpContext.Session.GetString("CURRENT_USER")}";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(Index));
        }

        // Trang thông báo khi không có quyền truy cập
        public IActionResult KhongCoQuyen(string? yeuCau)
        {
            ViewBag.YeuCau = yeuCau;
            ViewBag.CurrentRole = HttpContext.Session.GetString("CURRENT_ROLE") ?? "KHACH_HANG";
            return View();
        }
    }
}
