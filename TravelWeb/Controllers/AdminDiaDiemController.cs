using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Filters;
using TravelWeb.Models;

namespace TravelWeb.Controllers
{
    [PhanQuyen("ADMIN", "NHAN_VIEN")]
    public class AdminDiaDiemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminDiaDiemController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // 1. DANH SÁCH ĐỊA ĐIỂM
        public async Task<IActionResult> Index(string? tuKhoa, int? maDanhMuc)
        {
            var query = _context.DiaDiems
                .Include(d => d.DanhMuc)
                .Include(d => d.DichVus)
                .AsQueryable();

            if (!string.IsNullOrEmpty(tuKhoa))
            {
                query = query.Where(d => d.TenDiaDiem.Contains(tuKhoa) || d.DiaChi.Contains(tuKhoa));
            }

            if (maDanhMuc.HasValue && maDanhMuc.Value > 0)
            {
                query = query.Where(d => d.MaDanhMuc == maDanhMuc.Value);
            }

            var list = await query.OrderByDescending(d => d.MaDiaDiem).ToListAsync();
            ViewBag.DanhMucs = new SelectList(await _context.DanhMucs.ToListAsync(), "MaDanhMuc", "TenDanhMuc", maDanhMuc);
            ViewBag.TuKhoa = tuKhoa;
            return View(list);
        }

        // 2. THÊM MỚI ĐỊA ĐIỂM (GET)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.DanhMucs = new SelectList(await _context.DanhMucs.ToListAsync(), "MaDanhMuc", "TenDanhMuc");
            // Danh sách ảnh có sẵn trong thư mục images
            ViewBag.AvailableImages = GetAvailableImages();
            return View(new DiaDiem { TrangThai = "HOAT_DONG" });
        }

        // 3. THÊM MỚI ĐỊA ĐIỂM (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DiaDiem model, IFormFile? fileAnh, string? anhCoSan)
        {
            if (fileAnh != null && fileAnh.Length > 0)
            {
                // Upload ảnh mới
                string fileName = Guid.NewGuid().ToString("N").Substring(0, 8) + "_" + Path.GetFileName(fileAnh.FileName);
                string uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                Directory.CreateDirectory(uploadsFolder);
                string filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fileAnh.CopyToAsync(stream);
                }
                model.HinhAnh = fileName;
            }
            else if (!string.IsNullOrEmpty(anhCoSan))
            {
                model.HinhAnh = anhCoSan;
            }

            if (ModelState.IsValid)
            {
                _context.DiaDiems.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã thêm thành công địa điểm: {model.TenDiaDiem}";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.DanhMucs = new SelectList(await _context.DanhMucs.ToListAsync(), "MaDanhMuc", "TenDanhMuc", model.MaDanhMuc);
            ViewBag.AvailableImages = GetAvailableImages();
            return View(model);
        }

        // 4. CHỈNH SỬA ĐỊA ĐIỂM (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var diaDiem = await _context.DiaDiems.FindAsync(id);
            if (diaDiem == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy địa điểm yêu cầu.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.DanhMucs = new SelectList(await _context.DanhMucs.ToListAsync(), "MaDanhMuc", "TenDanhMuc", diaDiem.MaDanhMuc);
            ViewBag.AvailableImages = GetAvailableImages();
            return View(diaDiem);
        }

        // 5. CHỈNH SỬA ĐỊA ĐIỂM (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DiaDiem model, IFormFile? fileAnh, string? anhCoSan)
        {
            if (id != model.MaDiaDiem)
            {
                return NotFound();
            }

            if (fileAnh != null && fileAnh.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString("N").Substring(0, 8) + "_" + Path.GetFileName(fileAnh.FileName);
                string uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                Directory.CreateDirectory(uploadsFolder);
                string filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fileAnh.CopyToAsync(stream);
                }
                model.HinhAnh = fileName;
            }
            else if (!string.IsNullOrEmpty(anhCoSan))
            {
                model.HinhAnh = anhCoSan;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Cập nhật thành công địa điểm: {model.TenDiaDiem}";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.DiaDiems.AnyAsync(e => e.MaDiaDiem == model.MaDiaDiem))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            ViewBag.DanhMucs = new SelectList(await _context.DanhMucs.ToListAsync(), "MaDanhMuc", "TenDanhMuc", model.MaDanhMuc);
            ViewBag.AvailableImages = GetAvailableImages();
            return View(model);
        }

        // 6. THAY ĐỔI TRẠNG THÁI HOẠT ĐỘNG HOẶC XÓA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var diaDiem = await _context.DiaDiems.FindAsync(id);
            if (diaDiem == null) return NotFound();

            diaDiem.TrangThai = (diaDiem.TrangThai == "HOAT_DONG") ? "DONG_CUA" : "HOAT_DONG";
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã chuyển trạng thái địa điểm {diaDiem.TenDiaDiem} sang: {diaDiem.TrangThai}";
            return RedirectToAction(nameof(Index));
        }

        // 7. XÓA ĐỊA ĐIỂM (Kiểm tra ràng buộc khóa ngoại)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var diaDiem = await _context.DiaDiems
                .Include(d => d.DichVus)
                .FirstOrDefaultAsync(d => d.MaDiaDiem == id);

            if (diaDiem == null) return NotFound();

            if (diaDiem.DichVus.Any())
            {
                TempData["ErrorMessage"] = $"Không thể xóa địa điểm '{diaDiem.TenDiaDiem}' vì đang có {diaDiem.DichVus.Count} tour du lịch liên kết. Bạn có thể chuyển trạng thái sang Đóng cửa.";
                return RedirectToAction(nameof(Index));
            }

            _context.DiaDiems.Remove(diaDiem);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã xóa thành công địa điểm: {diaDiem.TenDiaDiem}";
            return RedirectToAction(nameof(Index));
        }

        // Hàm lấy danh sách tên file ảnh có sẵn trong thư mục wwwroot/images
        private List<string> GetAvailableImages()
        {
            var list = new List<string>();
            try
            {
                string imagesPath = Path.Combine(_env.WebRootPath, "images");
                if (Directory.Exists(imagesPath))
                {
                    var files = Directory.GetFiles(imagesPath);
                    foreach (var f in files)
                    {
                        string ext = Path.GetExtension(f).ToLower();
                        if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".webp" || ext == ".avif")
                        {
                            list.Add(Path.GetFileName(f));
                        }
                    }
                }
            }
            catch { }
            return list;
        }
    }
}
