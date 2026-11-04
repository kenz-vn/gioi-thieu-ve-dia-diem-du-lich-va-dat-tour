using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Filters;
using TravelWeb.Models;

namespace TravelWeb.Controllers
{
    [PhanQuyen("ADMIN", "NHAN_VIEN")]
    public class AdminNhaCungCapController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminNhaCungCapController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH NHÀ CUNG CẤP
        public async Task<IActionResult> Index(string? tuKhoa, string? loaiDichVu, string? trangThai)
        {
            var query = _context.NhaCungCaps.AsQueryable();

            if (!string.IsNullOrEmpty(tuKhoa))
            {
                query = query.Where(n => n.TenNhaCungCap.Contains(tuKhoa) ||
                                         (n.NguoiLienHe != null && n.NguoiLienHe.Contains(tuKhoa)) ||
                                         (n.SoDienThoai != null && n.SoDienThoai.Contains(tuKhoa)));
            }

            if (!string.IsNullOrEmpty(loaiDichVu))
            {
                query = query.Where(n => n.LoaiDichVu == loaiDichVu);
            }

            if (!string.IsNullOrEmpty(trangThai))
            {
                query = query.Where(n => n.TrangThai == trangThai);
            }

            ViewBag.TuKhoa = tuKhoa;
            ViewBag.LoaiDichVu = loaiDichVu;
            ViewBag.TrangThai = trangThai;
            ViewBag.TongNCC = await _context.NhaCungCaps.CountAsync();
            ViewBag.DangHopTac = await _context.NhaCungCaps.CountAsync(n => n.TrangThai == "DANG_HOP_TAC");

            var list = await query.OrderByDescending(n => n.MaNhaCungCap).ToListAsync();
            return View(list);
        }

        // 2. THÊM MỚI NHÀ CUNG CẤP (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View(new NhaCungCap { TrangThai = "DANG_HOP_TAC", LoaiDichVu = "Khách sạn / Lưu trú" });
        }

        // 3. THÊM MỚI NHÀ CUNG CẤP (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhaCungCap model)
        {
            if (ModelState.IsValid)
            {
                _context.NhaCungCaps.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã thêm thành công nhà cung cấp: {model.TenNhaCungCap}";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // 4. CHỈNH SỬA NHÀ CUNG CẤP (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var ncc = await _context.NhaCungCaps.FindAsync(id);
            if (ncc == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhà cung cấp.";
                return RedirectToAction(nameof(Index));
            }
            return View(ncc);
        }

        // 5. CHỈNH SỬA NHÀ CUNG CẤP (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NhaCungCap model)
        {
            if (id != model.MaNhaCungCap) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Cập nhật thành công nhà cung cấp: {model.TenNhaCungCap}";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.NhaCungCaps.AnyAsync(e => e.MaNhaCungCap == model.MaNhaCungCap)) return NotFound();
                    throw;
                }
            }
            return View(model);
        }

        // 6. CHUYỂN TRẠNG THÁI HỢP TÁC (Toggle)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleTrangThai(int id)
        {
            var ncc = await _context.NhaCungCaps.FindAsync(id);
            if (ncc == null) return NotFound();

            ncc.TrangThai = (ncc.TrangThai == "DANG_HOP_TAC") ? "TAM_DUNG" : "DANG_HOP_TAC";
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã đổi trạng thái nhà cung cấp '{ncc.TenNhaCungCap}' sang: {ncc.TrangThai}";
            return RedirectToAction(nameof(Index));
        }

        // 7. XÓA NHÀ CUNG CẤP
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var ncc = await _context.NhaCungCaps.FindAsync(id);
            if (ncc == null) return NotFound();

            _context.NhaCungCaps.Remove(ncc);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã xóa nhà cung cấp: {ncc.TenNhaCungCap}";
            return RedirectToAction(nameof(Index));
        }
    }
}
