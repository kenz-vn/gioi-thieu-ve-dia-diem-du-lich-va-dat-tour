using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Models;

namespace TravelWeb.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Báo cáo số đơn, doanh thu tiền cọc và tour phổ biến
        public async Task<IActionResult> Index(DateTime? tuNgay, DateTime? denNgay)
        {
            // 1. Xác định khoảng thời gian lọc (Mặc định: 30 ngày gần nhất)
            DateTime startDate = tuNgay ?? DateTime.Now.AddDays(-30);
            DateTime endDate = denNgay ?? DateTime.Now;

            // Lấy danh sách các đơn đặt lịch hợp lệ (trừ các đơn bị hủy)
            var query = _context.DatLichs
                .Include(d => d.DichVu)
                .Where(d => d.TrangThaiDat != "DA_HUY" && d.ThoiGianTao >= startDate && d.ThoiGianTao <= endDate);

            // 2. Thống kê doanh thu tiền cọc và số đơn theo ngày
            var baoCaoTheoNgay = await query
                .Where(d => d.ThoiGianTao.HasValue)
                .GroupBy(d => d.ThoiGianTao!.Value.Date)
                .Select(g => new BaoCaoDoanhThuViewModel
                {
                    NgayThongKe = g.Key,
                    TongSoDon = g.Count(),
                    TongSoKhach = g.Sum(x => x.SoLuongKhach),
                    TongDoanhThuCoc = g.Sum(x => x.SoTienCoc)
                })
                .OrderByDescending(x => x.NgayThongKe)
                .ToListAsync();

            // 3. Thống kê Tour phổ biến (Top các tour có số lượt đặt cao nhất)
            var tourPhoBien = await query
                .GroupBy(d => new { d.MaDichVu, TenTour = d.DichVu != null ? d.DichVu.TenDichVu : "Chưa xác định" })
                .Select(g => new TourPhoBienViewModel
                {
                    TenTour = g.Key.TenTour,
                    SoLuotDat = g.Count(),
                    TongKhach = g.Sum(x => x.SoLuongKhach),
                    TongDoanhThuCoc = g.Sum(x => x.SoTienCoc)
                })
                .OrderByDescending(x => x.SoLuotDat)
                .ThenByDescending(x => x.TongDoanhThuCoc)
                .Take(5) // Lấy Top 5 tour phổ biến
                .ToListAsync();

            // 4. Tổng hợp số liệu lên thẻ Dashboard
            ViewBag.TuNgay = startDate.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = endDate.ToString("yyyy-MM-dd");
            ViewBag.TongDon = baoCaoTheoNgay.Sum(x => x.TongSoDon);
            ViewBag.TongKhach = baoCaoTheoNgay.Sum(x => x.TongSoKhach);
            ViewBag.TongDoanhThu = baoCaoTheoNgay.Sum(x => x.TongDoanhThuCoc);
            ViewBag.TourPhoBienList = tourPhoBien;

            return View(baoCaoTheoNgay);
        }
    }

    // Class ViewModel phụ trợ
    public class BaoCaoDoanhThuViewModel
    {
        public DateTime NgayThongKe { get; set; }
        public int TongSoDon { get; set; }
        public int TongSoKhach { get; set; }
        public decimal TongDoanhThuCoc { get; set; }
    }

    public class TourPhoBienViewModel
    {
        public string TenTour { get; set; } = string.Empty;
        public int SoLuotDat { get; set; }
        public int TongKhach { get; set; }
        public decimal TongDoanhThuCoc { get; set; }
    }
}
