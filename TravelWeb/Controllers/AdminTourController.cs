using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Filters;
using TravelWeb.Models;

namespace TravelWeb.Controllers
{
    [PhanQuyen("ADMIN", "NHAN_VIEN")]
    public class AdminTourController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminTourController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // 1. DANH SÁCH TOUR
        public async Task<IActionResult> Index(string? tuKhoa, int? maDiaDiem, string? trangThai)
        {
            var query = _context.DichVus
                .Include(d => d.DiaDiem)
                .AsQueryable();

            if (!string.IsNullOrEmpty(tuKhoa))
            {
                query = query.Where(d => d.TenDichVu.Contains(tuKhoa) || (d.DiaDiem != null && d.DiaDiem.TenDiaDiem.Contains(tuKhoa)));
            }

            if (maDiaDiem.HasValue && maDiaDiem.Value > 0)
            {
                query = query.Where(d => d.MaDiaDiem == maDiaDiem.Value);
            }

            if (!string.IsNullOrEmpty(trangThai))
            {
                query = query.Where(d => d.TrangThaiTour == trangThai);
            }

            var tours = await query.OrderByDescending(d => d.MaDichVu).ToListAsync();

            ViewBag.DiaDiems = new SelectList(await _context.DiaDiems.ToListAsync(), "MaDiaDiem", "TenDiaDiem", maDiaDiem);
            ViewBag.TuKhoa = tuKhoa;
            ViewBag.TrangThai = trangThai;

            return View(tours);
        }

        // 2. THÊM MỚI TOUR (GET)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.DiaDiems = new SelectList(await _context.DiaDiems.ToListAsync(), "MaDiaDiem", "TenDiaDiem");
            ViewBag.AvailableImages = GetAvailableImages();
            return View(new DichVu { SoChoToiDa = 30, SoChoConLai = 30, TrangThaiTour = "DANG_BAN", TyLeCoc = 30 });
        }

        // 3. THÊM MỚI TOUR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DichVu model, IFormFile? fileAnh, string? anhCoSan)
        {
            // NGHIỆP VỤ BẮT BUỘC THEO TIÊU CHÍ DL-13:
            // 1. Kiểm tra số chỗ còn lại không được âm
            if (model.SoChoConLai < 0)
            {
                ModelState.AddModelError(nameof(model.SoChoConLai), "Tiêu chí bắt buộc: Số chỗ còn lại không được phép âm!");
            }

            // 2. Số chỗ còn lại không được lớn hơn số chỗ tối đa
            if (model.SoChoConLai > model.SoChoToiDa)
            {
                ModelState.AddModelError(nameof(model.SoChoConLai), "Số chỗ còn lại không thể lớn hơn số chỗ tối đa của tour!");
            }

            // 3. Nghiệp vụ Khuyến mãi: Giá khuyến mãi phải nhỏ hơn hoặc bằng giá gốc
            if (model.GiaKhuyenMai.HasValue && model.GiaKhuyenMai.Value >= model.GiaTien)
            {
                ModelState.AddModelError(nameof(model.GiaKhuyenMai), "Giá khuyến mãi phải nhỏ hơn giá gốc của tour!");
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
                model.HinhAnhPhong = fileName;
            }
            else if (!string.IsNullOrEmpty(anhCoSan))
            {
                model.HinhAnhPhong = anhCoSan;
            }

            // Tự động chuyển trạng thái HET_CHO nếu số chỗ còn lại = 0
            if (model.SoChoConLai == 0 && model.TrangThaiTour == "DANG_BAN")
            {
                model.TrangThaiTour = "HET_CHO";
            }

            if (ModelState.IsValid)
            {
                _context.DichVus.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã thêm mới tour thành công: {model.TenDichVu}";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.DiaDiems = new SelectList(await _context.DiaDiems.ToListAsync(), "MaDiaDiem", "TenDiaDiem", model.MaDiaDiem);
            ViewBag.AvailableImages = GetAvailableImages();
            return View(model);
        }

        // 4. CHỈNH SỬA TOUR (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var tour = await _context.DichVus.FindAsync(id);
            if (tour == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tour yêu cầu.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.DiaDiems = new SelectList(await _context.DiaDiems.ToListAsync(), "MaDiaDiem", "TenDiaDiem", tour.MaDiaDiem);
            ViewBag.AvailableImages = GetAvailableImages();
            return View(tour);
        }

        // 5. CHỈNH SỬA TOUR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DichVu model, IFormFile? fileAnh, string? anhCoSan)
        {
            if (id != model.MaDichVu) return NotFound();

            // NGHIỆP VỤ BẮT BUỘC THEO TIÊU CHÍ DL-13:
            if (model.SoChoConLai < 0)
            {
                ModelState.AddModelError(nameof(model.SoChoConLai), "Tiêu chí bắt buộc: Số chỗ còn lại không được phép âm!");
            }

            if (model.SoChoConLai > model.SoChoToiDa)
            {
                ModelState.AddModelError(nameof(model.SoChoConLai), "Số chỗ còn lại không thể lớn hơn số chỗ tối đa!");
            }

            if (model.GiaKhuyenMai.HasValue && model.GiaKhuyenMai.Value >= model.GiaTien)
            {
                ModelState.AddModelError(nameof(model.GiaKhuyenMai), "Giá khuyến mãi phải nhỏ hơn giá gốc của tour!");
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
                model.HinhAnhPhong = fileName;
            }
            else if (!string.IsNullOrEmpty(anhCoSan))
            {
                model.HinhAnhPhong = anhCoSan;
            }

            // Tự động chuyển HET_CHO nếu số chỗ = 0
            if (model.SoChoConLai == 0 && model.TrangThaiTour == "DANG_BAN")
            {
                model.TrangThaiTour = "HET_CHO";
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Cập nhật thành công tour: {model.TenDichVu}";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.DichVus.AnyAsync(e => e.MaDichVu == model.MaDichVu)) return NotFound();
                    throw;
                }
            }

            ViewBag.DiaDiems = new SelectList(await _context.DiaDiems.ToListAsync(), "MaDiaDiem", "TenDiaDiem", model.MaDiaDiem);
            ViewBag.AvailableImages = GetAvailableImages();
            return View(model);
        }

        // 6. NGỪNG TOUR / MỞ LẠI TOUR (Toggle Status)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleTrangThai(int id)
        {
            var tour = await _context.DichVus.FindAsync(id);
            if (tour == null) return NotFound();

            if (tour.TrangThaiTour == "DANG_BAN")
            {
                tour.TrangThaiTour = "TAM_NGUNG";
                TempData["SuccessMessage"] = $"Đã tạm ngừng tour: {tour.TenDichVu}";
            }
            else
            {
                tour.TrangThaiTour = (tour.SoChoConLai > 0) ? "DANG_BAN" : "HET_CHO";
                TempData["SuccessMessage"] = $"Đã kích hoạt mở bán lại tour: {tour.TenDichVu}";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 7. CẬP NHẬT NHANH SỐ CHỖ CÒN LẠI (Kiểm tra chặn âm nghiêm ngặt)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatSoCho(int id, int soChoConLai)
        {
            if (soChoConLai < 0)
            {
                TempData["ErrorMessage"] = "Lỗi nghiệp vụ: Số chỗ còn lại không được phép nhỏ hơn 0!";
                return RedirectToAction(nameof(Index));
            }

            var tour = await _context.DichVus.FindAsync(id);
            if (tour == null) return NotFound();

            tour.SoChoConLai = soChoConLai;
            if (soChoConLai == 0 && tour.TrangThaiTour == "DANG_BAN")
            {
                tour.TrangThaiTour = "HET_CHO";
            }
            else if (soChoConLai > 0 && tour.TrangThaiTour == "HET_CHO")
            {
                tour.TrangThaiTour = "DANG_BAN";
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã cập nhật số chỗ còn lại cho tour '{tour.TenDichVu}': {soChoConLai} chỗ.";
            return RedirectToAction(nameof(Index));
        }

        // 8. XÓA TOUR (Kiểm tra ràng buộc đơn đặt)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var tour = await _context.DichVus
                .Include(d => d.DatLichs)
                .FirstOrDefaultAsync(d => d.MaDichVu == id);

            if (tour == null) return NotFound();

            if (tour.DatLichs.Any())
            {
                TempData["ErrorMessage"] = $"Không thể xóa tour '{tour.TenDichVu}' vì đã có {tour.DatLichs.Count} đơn đặt lịch liên kết. Bạn hãy chọn Tạm ngừng nhận khách.";
                return RedirectToAction(nameof(Index));
            }

            _context.DichVus.Remove(tour);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã xóa tour thành công: {tour.TenDichVu}";
            return RedirectToAction(nameof(Index));
        }

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
