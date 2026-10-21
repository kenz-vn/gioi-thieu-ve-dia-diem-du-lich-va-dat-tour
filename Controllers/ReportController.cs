using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        // UC-06: BÁO CÁO THỐNG KÊ ĐẶT TOUR (GET)
        public async Task<IActionResult> Index(DateTime? tuNgay, DateTime? denNgay)
        {
            // Điểm rẽ nhánh 1: Kiểm tra khoảng thời gian (E1)
            DateTime startDate = tuNgay ?? DateTime.Now.AddDays(-30);
            DateTime endDate = denNgay ?? DateTime.Now;

            if (startDate > endDate)
            {
                TempData["ErrorMessage"] = "Khoảng thời gian không hợp lệ (E1): Ngày bắt đầu không được lớn hơn ngày kết thúc.";
                startDate = DateTime.Now.AddDays(-30);
                endDate = DateTime.Now;
            }

            var query = _context.DatLichs
                .Include(d => d.DichVu)
                .Where(d => d.ThoiGianTao >= startDate && d.ThoiGianTao <= endDate.AddDays(1).AddTicks(-1));

            // Thống kê tổng hợp theo trạng thái
            var allOrders = await query.ToListAsync();

            // Điểm rẽ nhánh 2: Kiểm tra dữ liệu (E2)
            if (!allOrders.Any())
            {
                TempData["InfoMessage"] = "Không có dữ liệu đơn đặt tour trong khoảng thời gian đã chọn (E2).";
            }

            // Thống kê số đơn theo trạng thái (UC-06)
            ViewBag.TongDonTatCa   = allOrders.Count;
            ViewBag.DonChoXacNhan  = allOrders.Count(d => d.TrangThaiDat == "CHO_XAC_NHAN");
            ViewBag.DonDaXacNhan   = allOrders.Count(d => d.TrangThaiDat == "DA_XAC_NHAN");
            ViewBag.DonTuChoi      = allOrders.Count(d => d.TrangThaiDat == "TU_CHOI");
            ViewBag.DonDaHuy       = allOrders.Count(d => d.TrangThaiDat == "DA_HUY");

            // Thống kê doanh thu tiền cọc theo ngày
            var baoCaoTheoNgay = allOrders
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
                .ToList();

            // Top 5 tour phổ biến nhất
            var tourPhoBien = allOrders
                .Where(d => d.TrangThaiDat != "DA_HUY" && d.TrangThaiDat != "TU_CHOI")
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
                .Take(5)
                .ToList();

            ViewBag.TuNgay = startDate.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = endDate.ToString("yyyy-MM-dd");
            ViewBag.TongDon = baoCaoTheoNgay.Sum(x => x.TongSoDon);
            ViewBag.TongKhach = baoCaoTheoNgay.Sum(x => x.TongSoKhach);
            ViewBag.TongDoanhThu = baoCaoTheoNgay.Sum(x => x.TongDoanhThuCoc);
            ViewBag.TourPhoBienList = tourPhoBien;

            return View(baoCaoTheoNgay);
        }

        // UC-06: XUẤT BÁO CÁO CSV (GET)
        public async Task<IActionResult> XuatCSV(DateTime? tuNgay, DateTime? denNgay)
        {
            DateTime startDate = tuNgay ?? DateTime.Now.AddDays(-30);
            DateTime endDate = denNgay ?? DateTime.Now;

            if (startDate > endDate)
            {
                TempData["ErrorMessage"] = "Khoảng thời gian không hợp lệ. Không thể xuất báo cáo.";
                return RedirectToAction(nameof(Index));
            }

            var orders = await _context.DatLichs
                .Include(d => d.DichVu)
                .ThenInclude(dv => dv!.DiaDiem)
                .Where(d => d.ThoiGianTao >= startDate && d.ThoiGianTao <= endDate.AddDays(1).AddTicks(-1))
                .OrderByDescending(d => d.ThoiGianTao)
                .ToListAsync();

            // Điểm rẽ nhánh 3: Kiểm tra kết quả xuất file (E3)
            if (!orders.Any())
            {
                TempData["ErrorMessage"] = "Không có dữ liệu để xuất báo cáo trong khoảng thời gian đã chọn.";
                return RedirectToAction(nameof(Index), new { tuNgay, denNgay });
            }

            try
            {
                var sb = new StringBuilder();
                // Header CSV
                sb.AppendLine("Mã Tra Cứu,Mã Đặt Lịch,Tên Tour,Địa Điểm,Họ Tên Khách,Số ĐT,Email,Ngày Đặt,Ngày Khởi Hành,Số Khách,Tiền Cọc (VNĐ),Trạng Thái");

                foreach (var d in orders)
                {
                    string trangThai = d.TrangThaiDat switch
                    {
                        "CHO_XAC_NHAN" => "Chờ xác nhận",
                        "DA_XAC_NHAN"  => "Đã xác nhận",
                        "TU_CHOI"      => "Từ chối",
                        "DA_HUY"       => "Đã hủy",
                        "HOAN_THANH"   => "Hoàn thành",
                        _              => d.TrangThaiDat ?? ""
                    };

                    sb.AppendLine(string.Join(",",
                        $"\"{d.MaTraCuu}\"",
                        $"\"{d.MaDatLich}\"",
                        $"\"{d.DichVu?.TenDichVu ?? ""}\"",
                        $"\"{d.DichVu?.DiaDiem?.TenDiaDiem ?? ""}\"",
                        $"\"{d.HoTen ?? ""}\"",
                        $"\"{d.SoDienthoai ?? ""}\"",
                        $"\"{d.Email ?? ""}\"",
                        $"\"{d.ThoiGianTao?.ToString("dd/MM/yyyy HH:mm") ?? ""}\"",
                        $"\"{d.NgayThamQuan:dd/MM/yyyy}\"",
                        d.SoLuongKhach,
                        d.SoTienCoc.ToString("N0"),
                        $"\"{trangThai}\""
                    ));
                }

                byte[] fileBytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
                string fileName = $"BaoCaoDatTour_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.csv";

                // Xuất báo cáo thành công
                return File(fileBytes, "text/csv; charset=utf-8", fileName);
            }
            catch
            {
                // E3: Tạo file thất bại
                TempData["ErrorMessage"] = "Không thể tạo báo cáo (E3). Vui lòng thử lại sau.";
                return RedirectToAction(nameof(Index), new { tuNgay, denNgay });
            }
        }
    }

    // ViewModel phụ trợ
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
