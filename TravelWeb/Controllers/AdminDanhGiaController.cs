using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Filters;
using TravelWeb.Models;

namespace TravelWeb.Controllers
{
    [PhanQuyen("ADMIN", "NHAN_VIEN")]
    public class AdminDanhGiaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDanhGiaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH ĐÁNH GIÁ CẦN KIỂM DUYỆT
        public async Task<IActionResult> Index(int? daDuyet, int? diemDanhGia)
        {
            var query = _context.DanhGias
                .Include(d => d.DiaDiem)
                .Include(d => d.KhachHang)
                .Include(d => d.DatLich)
                .AsQueryable();

            if (daDuyet.HasValue)
            {
                query = query.Where(d => d.DaDuyet == daDuyet.Value);
            }

            if (diemDanhGia.HasValue && diemDanhGia.Value > 0)
            {
                query = query.Where(d => d.DiemDanhGia == diemDanhGia.Value);
            }

            ViewBag.CountChoDuyet = await _context.DanhGias.CountAsync(d => d.DaDuyet == 0);
            ViewBag.CountDaDuyet = await _context.DanhGias.CountAsync(d => d.DaDuyet == 1);
            ViewBag.CountTuChoi = await _context.DanhGias.CountAsync(d => d.DaDuyet == -1);
            ViewBag.DaDuyet = daDuyet;
            ViewBag.DiemDanhGia = diemDanhGia;

            var danhSach = await query.OrderByDescending(d => d.MaDanhGia).ToListAsync();
            return View(danhSach);
        }

        // 2. DUYỆT CÔNG KHAI ĐÁNH GIÁ (DaDuyet = 1)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duyet(int id)
        {
            var danhGia = await _context.DanhGias.FindAsync(id);
            if (danhGia == null) return NotFound();

            danhGia.DaDuyet = 1; // Đã duyệt
            danhGia.GhiChuKiemDuyet = "Đã kiểm duyệt: Nội dung lịch sự, hợp lệ.";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã duyệt công khai đánh giá #{id} thành công!";
            return RedirectToAction(nameof(Index));
        }

        // 3. TỪ CHỐI / ẨN ĐÁNH GIÁ (DaDuyet = -1)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TuChoi(int id, string lyDo)
        {
            var danhGia = await _context.DanhGias.FindAsync(id);
            if (danhGia == null) return NotFound();

            if (string.IsNullOrWhiteSpace(lyDo))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập lý do ẩn/từ chối đánh giá!";
                return RedirectToAction(nameof(Index));
            }

            danhGia.DaDuyet = -1; // Từ chối / Ẩn
            danhGia.GhiChuKiemDuyet = lyDo.Trim();
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã ẩn đánh giá #{id} khỏi hệ thống. Lý do: {lyDo}";
            return RedirectToAction(nameof(Index));
        }

        // 4. PHẢN HỒI ĐÁNH GIÁ CỦA KHÁCH HÀNG (Không sửa nội dung khách)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PhanHoi(int id, string noiDungPhanHoi)
        {
            var danhGia = await _context.DanhGias.FindAsync(id);
            if (danhGia == null) return NotFound();

            if (string.IsNullOrWhiteSpace(noiDungPhanHoi))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập nội dung phản hồi khách hàng!";
                return RedirectToAction(nameof(Index));
            }

            // Ghi nhận phản hồi của nhân viên - GIỮ NGUYÊN BÌNH LUẬN VÀ ĐIỂM SỐ CỦA KHÁCH THEO TIÊU CHÍ
            danhGia.PhanHoiCuaNhanVien = noiDungPhanHoi.Trim();
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã gửi phản hồi cho đánh giá #{id} thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
