using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Models;

namespace TravelWeb.Controllers
{
    public class TourController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TourController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Tour/ or /Tour/Index
        public async Task<IActionResult> Index(string? tuKhoa, int? maDanhMuc, int pageNumber = 1, int pageSize = 6)
        {
            var query = _context.DichVus
                .Include(d => d.DiaDiem)
                .ThenInclude(dd => dd!.DanhMuc)
                .AsQueryable();

            if (!string.IsNullOrEmpty(tuKhoa))
            {
                query = query.Where(d => d.TenDichVu.Contains(tuKhoa) || (d.DiaDiem != null && d.DiaDiem.TenDiaDiem.Contains(tuKhoa)));
            }

            if (maDanhMuc.HasValue && maDanhMuc.Value > 0)
            {
                query = query.Where(d => d.DiaDiem != null && d.DiaDiem.MaDanhMuc == maDanhMuc.Value);
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var tours = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TuKhoa = tuKhoa;
            ViewBag.MaDanhMuc = maDanhMuc;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.DanhMucs = await _context.DanhMucs.ToListAsync();

            return View(tours);
        }

        // GET: /Tour/Details/5
        public IActionResult Details(int id)
        {
            return RedirectToAction("DatTour", "Booking", new { id });
        }
    }
}
