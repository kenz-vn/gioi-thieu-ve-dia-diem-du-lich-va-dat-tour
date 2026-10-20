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

        // 1. HIỂN THỊ TRANG ĐẶT TOUR (GET)
        [HttpGet]
        public async Task<IActionResult> DatTour(int id) // id ở đây là ma_dich_vu của tour
        {
            var dichVu = await _context.DichVus
                .Include(d => d.DiaDiem)
                .FirstOrDefaultAsync(d => d.MaDichVu == id);

            if (dichVu == null)
            {
                return NotFound();
            }

            return View(dichVu);
        }

        // 2. XỬ LÝ LƯU TRỮ ĐƠN ĐẶT & SINH MÃ ĐẶT TOUR (POST) - Task DL-17
        [HttpPost]
        public async Task<IActionResult> XacNhanDatTour(int maDichVu, int soLuongKhach, DateTime ngayThamQuan, string hoTen, string email, string soDienthoai)
        {
            var dichVu = await _context.DichVus.FindAsync(maDichVu);
            if (dichVu == null) return NotFound();

            // Tính toán tiền cọc
            decimal tongTien = dichVu.GiaTien * soLuongKhach;
            decimal soTienCoc = tongTien * (dichVu.TyLeCoc / 100);

            // Sinh mã tra cứu công khai và mã đặt lịch duy nhất
            string maTraCuu = "TOUR-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(1000, 9999);
            string maDatLich = "BK" + DateTime.Now.Ticks.ToString().Substring(10);

            // Tạo đối tượng lưu vào bảng DAT_LICH trong SQL Server
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
                MaKhachHang = 2 // Giả lập ID khách hàng
            };

            _context.DatLichs.Add(datLich);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đặt tour thành công! Mã tra cứu của bạn là: {maTraCuu}";
            return RedirectToAction("LichSuDatTour", new { maTraCuu = maTraCuu });
        }

        // 3. XEM LỊCH SỬ ĐẶT TOUR & TRA CỨU ĐƠN (GET) - Task DL-17
        [HttpGet]
        public async Task<IActionResult> LichSuDatTour(string? maTraCuu, int? maKhachHang)
        {
            var query = _context.DatLichs
                .Include(d => d.DichVu)
                .ThenInclude(dv => dv!.DiaDiem)
                .AsQueryable();

            if (!string.IsNullOrEmpty(maTraCuu))
            {
                query = query.Where(d => d.MaTraCuu == maTraCuu);
            }
            else
            {
                // Mặc định lọc theo khách hàng (ví dụ ma_khach_hang = 2)
                int khId = maKhachHang ?? 2;
                query = query.Where(d => d.MaKhachHang == khId);
            }

            var danhSachDon = await query.OrderByDescending(d => d.ThoiGianTao).ToListAsync();

            ViewBag.MaTraCuu = maTraCuu;
            return View(danhSachDon);
        }
    }
}
