using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Filters;
using TravelWeb.Models;

namespace TravelWeb.Controllers
{
    [PhanQuyen("ADMIN", "NHAN_VIEN")]
    public class AdminLienHeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminLienHeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH LIÊN HỆ
        public async Task<IActionResult> Index(string? trangThai, string? tuKhoa)
        {
            var query = _context.LienHes.AsQueryable();

            if (!string.IsNullOrEmpty(trangThai))
            {
                query = query.Where(l => l.TrangThai == trangThai);
            }

            if (!string.IsNullOrEmpty(tuKhoa))
            {
                query = query.Where(l => l.HoTen.Contains(tuKhoa) || l.Email.Contains(tuKhoa) || l.TieuDe.Contains(tuKhoa));
            }

            ViewBag.CountChuaXuLy = await _context.LienHes.CountAsync(l => l.TrangThai == "CHUA_XU_LY");
            ViewBag.CountDangXuLy = await _context.LienHes.CountAsync(l => l.TrangThai == "DANG_XU_LY");
            ViewBag.CountDaXuLy = await _context.LienHes.CountAsync(l => l.TrangThai == "DA_XU_LY");
            ViewBag.TrangThai = trangThai;
            ViewBag.TuKhoa = tuKhoa;

            var list = await query.OrderByDescending(l => l.NgayGui).ToListAsync();
            return View(list);
        }

        // 2. CHI TIẾT & FORM PHẢN HỒI (GET)
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var lienHe = await _context.LienHes.FindAsync(id);
            if (lienHe == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy yêu cầu liên hệ.";
                return RedirectToAction(nameof(Index));
            }

            // Nếu đang CHUA_XU_LY, tự động chuyển sang DANG_XU_LY khi nhân viên mở xem
            if (lienHe.TrangThai == "CHUA_XU_LY")
            {
                lienHe.TrangThai = "DANG_XU_LY";
                lienHe.NguoiXuLy = HttpContext.Session.GetString("CURRENT_USER") ?? "Bùi Trung Kiên (Nhân viên)";
                await _context.SaveChangesAsync();
            }

            return View(lienHe);
        }

        // 3. XỬ LÝ & PHẢN HỒI LIÊN HỆ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XuLy(int id, string phanHoi, string trangThai)
        {
            var lienHe = await _context.LienHes.FindAsync(id);
            if (lienHe == null) return NotFound();

            if (string.IsNullOrWhiteSpace(phanHoi))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập nội dung phản hồi khách hàng!";
                return RedirectToAction(nameof(Details), new { id });
            }

            string staffName = HttpContext.Session.GetString("CURRENT_USER") ?? "Bùi Trung Kiên (Nhân viên)";

            lienHe.PhanHoi = phanHoi.Trim();
            lienHe.TrangThai = string.IsNullOrEmpty(trangThai) ? "DA_XU_LY" : trangThai;
            lienHe.NguoiXuLy = staffName;
            lienHe.NgayXuLy = DateTime.Now;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã xử lý và gửi phản hồi cho khách hàng '{lienHe.HoTen}' thành công!";
            return RedirectToAction(nameof(Index));
        }

        // 4. XÓA TIN NHẮN RÁC / SPAM
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var lienHe = await _context.LienHes.FindAsync(id);
            if (lienHe == null) return NotFound();

            _context.LienHes.Remove(lienHe);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã xóa tin nhắn liên hệ.";
            return RedirectToAction(nameof(Index));
        }
    }
}
