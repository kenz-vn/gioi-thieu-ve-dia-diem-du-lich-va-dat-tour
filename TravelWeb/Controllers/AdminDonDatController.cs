using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Filters;
using TravelWeb.Models;

namespace TravelWeb.Controllers
{
    [PhanQuyen("ADMIN", "NHAN_VIEN")]
    public class AdminDonDatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDonDatController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH ĐƠN ĐẶT TOUR
        public async Task<IActionResult> Index(string? tuKhoa, string? trangThai, DateTime? tuNgay, DateTime? denNgay)
        {
            var query = _context.DatLichs
                .Include(d => d.DichVu)
                .ThenInclude(dv => dv!.DiaDiem)
                .AsQueryable();

            if (!string.IsNullOrEmpty(tuKhoa))
            {
                query = query.Where(d => d.MaDatLich.Contains(tuKhoa) ||
                                         d.MaTraCuu.Contains(tuKhoa) ||
                                         (d.HoTen != null && d.HoTen.Contains(tuKhoa)) ||
                                         (d.SoDienthoai != null && d.SoDienthoai.Contains(tuKhoa)));
            }

            if (!string.IsNullOrEmpty(trangThai))
            {
                query = query.Where(d => d.TrangThaiDat == trangThai);
            }

            if (tuNgay.HasValue)
            {
                query = query.Where(d => d.ThoiGianTao >= tuNgay.Value);
            }

            if (denNgay.HasValue)
            {
                DateTime end = denNgay.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(d => d.ThoiGianTao <= end);
            }

            // Thống kê nhanh theo trạng thái để hiển thị Tabs badge
            ViewBag.CountAll = await _context.DatLichs.CountAsync();
            ViewBag.CountChoXacNhan = await _context.DatLichs.CountAsync(d => d.TrangThaiDat == "CHO_XAC_NHAN");
            ViewBag.CountDaXacNhan = await _context.DatLichs.CountAsync(d => d.TrangThaiDat == "DA_XAC_NHAN");
            ViewBag.CountTuChoi = await _context.DatLichs.CountAsync(d => d.TrangThaiDat == "TU_CHOI");
            ViewBag.CountHoanThanh = await _context.DatLichs.CountAsync(d => d.TrangThaiDat == "HOAN_THANH");
            ViewBag.CountDaHuy = await _context.DatLichs.CountAsync(d => d.TrangThaiDat == "DA_HUY");

            ViewBag.TuKhoa = tuKhoa;
            ViewBag.TrangThai = trangThai;
            ViewBag.TuNgay = tuNgay?.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = denNgay?.ToString("yyyy-MM-dd");

            var danhSach = await query.OrderByDescending(d => d.ThoiGianTao).ToListAsync();
            return View(danhSach);
        }

        // 2. CHI TIẾT ĐƠN ĐẶT TOUR
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var don = await _context.DatLichs
                .Include(d => d.DichVu)
                .ThenInclude(dv => dv!.DiaDiem)
                .Include(d => d.ThanhToans)
                .Include(d => d.DanhGias)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (don == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn đặt tour.";
                return RedirectToAction(nameof(Index));
            }

            return View(don);
        }

        // 3. XÁC NHẬN ĐƠN (POST) - DL-19
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanDon(int id, string? ghiChu)
        {
            var don = await _context.DatLichs
                .Include(d => d.DichVu)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (don == null) return NotFound();

            // Kiểm tra quy tắc chuyển trạng thái
            if (don.TrangThaiDat != "CHO_XAC_NHAN")
            {
                TempData["ErrorMessage"] = $"Không thể xác nhận đơn ở trạng thái hiện tại ({don.TrangThaiDat}). Chỉ duyệt đơn đang 'CHO_XAC_NHAN'.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // Kiểm tra số chỗ còn lại của Tour
            if (don.DichVu != null)
            {
                if (don.DichVu.SoChoConLai < don.SoLuongKhach)
                {
                    TempData["ErrorMessage"] = $"Lỗi: Tour '{don.DichVu.TenDichVu}' chỉ còn {don.DichVu.SoChoConLai} chỗ, không đủ cho đơn đặt {don.SoLuongKhach} khách!";
                    return RedirectToAction(nameof(Details), new { id });
                }

                // Trừ số chỗ còn lại của Tour
                don.DichVu.SoChoConLai -= don.SoLuongKhach;
                if (don.DichVu.SoChoConLai == 0)
                {
                    don.DichVu.TrangThaiTour = "HET_CHO";
                }
            }

            string staffName = HttpContext.Session.GetString("CURRENT_USER") ?? "Bùi Trung Kiên (Nhân viên)";

            don.TrangThaiDat = "DA_XAC_NHAN";
            don.NguoiXuLy = staffName;
            don.ThoiGianXuLy = DateTime.Now;
            if (!string.IsNullOrEmpty(ghiChu))
            {
                don.GhiChuXuLy = ghiChu;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Xác nhận thành công đơn #{don.MaDatLich}! Đã ghi nhận người xử lý: {staffName}";
            return RedirectToAction(nameof(Details), new { id });
        }

        // 4. TỪ CHỐI ĐƠN (POST) - DL-19 (Bắt buộc có lý do từ chối)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TuChoiDon(int id, string lyDoTuChoi, string? ghiChu)
        {
            var don = await _context.DatLichs.FindAsync(id);
            if (don == null) return NotFound();

            // Tiêu chí nghiệm thu DL-19: BẮT BUỘC CÓ LÝ DO TỪ CHỐI
            if (string.IsNullOrWhiteSpace(lyDoTuChoi))
            {
                TempData["ErrorMessage"] = "Tiêu chí bắt buộc: Vui lòng nhập lý do từ chối để thông báo cho khách hàng!";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (don.TrangThaiDat != "CHO_XAC_NHAN")
            {
                TempData["ErrorMessage"] = "Chỉ có thể từ chối các đơn đang ở trạng thái 'Chờ xác nhận'.";
                return RedirectToAction(nameof(Details), new { id });
            }

            string staffName = HttpContext.Session.GetString("CURRENT_USER") ?? "Bùi Trung Kiên (Nhân viên)";

            don.TrangThaiDat = "TU_CHOI";
            don.LyDoTuChoi = lyDoTuChoi.Trim();
            don.NguoiXuLy = staffName;
            don.ThoiGianXuLy = DateTime.Now;
            if (!string.IsNullOrEmpty(ghiChu))
            {
                don.GhiChuXuLy = ghiChu;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã từ chối đơn #{don.MaDatLich} với lý do: '{lyDoTuChoi}'. Thao tác được ghi nhận thành công!";
            return RedirectToAction(nameof(Details), new { id });
        }

        // 5. CẬP NHẬT TRẠNG THÁI KHÁC (HOÀN THÀNH / HỦY) - DL-19
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(int id, string trangThaiMoi, string? ghiChu)
        {
            var don = await _context.DatLichs
                .Include(d => d.DichVu)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (don == null) return NotFound();

            string staffName = HttpContext.Session.GetString("CURRENT_USER") ?? "Bùi Trung Kiên (Nhân viên)";

            // Quy tắc chuyển trạng thái
            if (trangThaiMoi == "HOAN_THANH")
            {
                if (don.TrangThaiDat != "DA_XAC_NHAN")
                {
                    TempData["ErrorMessage"] = "Chỉ đơn 'Đã xác nhận' mới có thể chuyển sang 'Hoàn thành'.";
                    return RedirectToAction(nameof(Details), new { id });
                }
                don.TrangThaiDat = "HOAN_THANH";
            }
            else if (trangThaiMoi == "DA_HUY")
            {
                // Nếu đơn trước đó là DA_XAC_NHAN, hoàn trả lại số chỗ cho tour
                if (don.TrangThaiDat == "DA_XAC_NHAN" && don.DichVu != null)
                {
                    don.DichVu.SoChoConLai += don.SoLuongKhach;
                    if (don.DichVu.TrangThaiTour == "HET_CHO" && don.DichVu.SoChoConLai > 0)
                    {
                        don.DichVu.TrangThaiTour = "DANG_BAN";
                    }
                }
                don.TrangThaiDat = "DA_HUY";
            }

            don.NguoiXuLy = staffName;
            don.ThoiGianXuLy = DateTime.Now;
            if (!string.IsNullOrEmpty(ghiChu))
            {
                don.GhiChuXuLy = ghiChu;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã cập nhật trạng thái đơn #{don.MaDatLich} sang: {trangThaiMoi}.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
