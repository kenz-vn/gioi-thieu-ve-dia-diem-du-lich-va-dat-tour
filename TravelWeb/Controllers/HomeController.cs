using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TravelWeb.Models;

namespace TravelWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Trang chủ: hiển thị danh sách Tour, Tìm kiếm, Lọc và Phân trang
        public async Task<IActionResult> Index(string? tuKhoa, int? maDanhMuc, int pageNumber = 1, int pageSize = 6)
        {
            var query = _context.DichVus
                .Include(d => d.DiaDiem)
                .ThenInclude(dd => dd!.DanhMuc)
                .AsQueryable();

            // 1. Tìm kiếm theo từ khóa (tên dịch vụ/tour hoặc tên địa điểm)
            if (!string.IsNullOrEmpty(tuKhoa))
            {
                query = query.Where(d => d.TenDichVu.Contains(tuKhoa) || (d.DiaDiem != null && d.DiaDiem.TenDiaDiem.Contains(tuKhoa)));
            }

            // 2. Lọc theo danh mục địa điểm
            if (maDanhMuc.HasValue && maDanhMuc.Value > 0)
            {
                query = query.Where(d => d.DiaDiem != null && d.DiaDiem.MaDanhMuc == maDanhMuc.Value);
            }

            // 3. Tính toán phân trang
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var tours = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Truyền dữ liệu sang View qua ViewBag
            ViewBag.TuKhoa = tuKhoa;
            ViewBag.MaDanhMuc = maDanhMuc;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.DanhMucs = await _context.DanhMucs.ToListAsync();

            return View(tours);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
