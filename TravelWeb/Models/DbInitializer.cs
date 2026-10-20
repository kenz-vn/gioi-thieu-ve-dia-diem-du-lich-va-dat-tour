using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TravelWeb.Models
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            try
            {
                // Kiểm tra xem database và bảng DANH_GIA đã hoàn thiện chưa
                if (context.DanhGias.Any())
                {
                    return; // Đã có dữ liệu đầy đủ
                }
            }
            catch
            {
                // Nếu bị lỗi (do lần chạy trước DB bị tạo dở dang), xóa DB dở để tạo lại sạch sẽ
                try
                {
                    context.Database.EnsureDeleted();
                }
                catch { }
            }

            // Tự động tạo cơ sở dữ liệu và toàn bộ các bảng
            context.Database.EnsureCreated();

            // Nếu đã có Danh mục thì coi như DB đã được seed
            if (context.DanhMucs.Any())
            {
                return;
            }

            // 1. Thêm Danh mục
            var danhMucs = new DanhMuc[]
            {
                new DanhMuc { TenDanhMuc = "Biển Đảo Nghỉ Dưỡng" },
                new DanhMuc { TenDanhMuc = "Văn Hóa - Lịch Sử" },
                new DanhMuc { TenDanhMuc = "Miền Núi - Sinh Thái" },
                new DanhMuc { TenDanhMuc = "Khám Phá - Mạo Hiểm" }
            };
            context.DanhMucs.AddRange(danhMucs);
            context.SaveChanges();

            // 2. Thêm Tài khoản người dùng mẫu
            var khachHangs = new KhachHang[]
            {
                new KhachHang { HoTen = "Nguyễn Quản Trị", Email = "admin@travelweb.vn", SoDienthoai = "0988111222", VaiTro = "ADMIN" },
                new KhachHang { HoTen = "Bùi Trung Kiên", Email = "kien.bt@travelweb.vn", SoDienthoai = "0977222333", VaiTro = "NHAN_VIEN" },
                new KhachHang { HoTen = "Trần Minh Hoàng", Email = "khachhang@gmail.com", SoDienthoai = "0912345678", VaiTro = "KHACH_HANG" }
            };
            context.KhachHangs.AddRange(khachHangs);
            context.SaveChanges();

            // 3. Thêm Địa điểm du lịch (DL-09)
            var diaDiems = new DiaDiem[]
            {
                new DiaDiem
                {
                    TenDiaDiem = "Thành phố biển Đà Nẵng",
                    DiaChi = "Bán đảo Sơn Trà, Q. Sơn Trà, TP. Đà Nẵng",
                    MaDanhMuc = danhMucs[0].MaDanhMuc,
                    Hotline = "0236.3888.999",
                    TrangThai = "HOAT_DONG",
                    HinhAnh = "danang.jpg",
                    VanHoa = "Văn hóa giao thoa miền Trung, người dân thân thiện, lễ hội pháo hoa quốc tế rực rỡ.",
                    DoAn = "Mì Quảng, Bánh tráng cuốn thịt heo, Chả bò Đà Nẵng, Hải sản tươi sống Mỹ Khê.",
                    NoiCanToi = "Bà Nà Hills, Cầu Rồng, Ngũ Hành Sơn, Bán đảo Sơn Trà, Biển Mỹ Khê."
                },
                new DiaDiem
                {
                    TenDiaDiem = "Vịnh Hạ Long - Kỳ Quan Thế Giới",
                    DiaChi = "TP. Hạ Long, Tỉnh Quảng Ninh",
                    MaDanhMuc = danhMucs[0].MaDanhMuc,
                    Hotline = "0203.3666.888",
                    TrangThai = "HOAT_DONG",
                    HinhAnh = "halong.jpg",
                    VanHoa = "Văn hóa làng chài cổ xưa, di sản thiên nhiên thế giới 2 lần được UNESCO công nhận.",
                    DoAn = "Chả mực Hạ Long, Sá sùng, Sam biển, Bún hải sản, Bánh gật gù.",
                    NoiCanToi = "Đảo Ti Tốp, Hang Sửng Sốt, Động Thiên Cung, Vịnh Bái Tử Long."
                },
                new DiaDiem
                {
                    TenDiaDiem = "Đà Lạt - Xứ Sở Sương Mù",
                    DiaChi = "TP. Đà Lạt, Tỉnh Lâm Đồng",
                    MaDanhMuc = danhMucs[2].MaDanhMuc,
                    Hotline = "0263.3555.777",
                    TrangThai = "HOAT_DONG",
                    HinhAnh = "dalat.webp",
                    VanHoa = "Kiến trúc Pháp cổ kính xen lẫn không gian hoa cỏ lãng mạn cao nguyên.",
                    DoAn = "Lẩu gà lá é, Bánh tráng nướng, Kem bơ Thanh Thảo, Dâu tây Đà Lạt.",
                    NoiCanToi = "Hồ Xuân Hương, Thung Lũng Tình Yêu, Đỉnh Langbiang, Vườn hoa thành phố."
                },
                new DiaDiem
                {
                    TenDiaDiem = "Sapa - Thị Trấn Trong Mây",
                    DiaChi = "TX. Sapa, Tỉnh Lào Cai",
                    MaDanhMuc = danhMucs[2].MaDanhMuc,
                    Hotline = "0214.3777.666",
                    TrangThai = "HOAT_DONG",
                    HinhAnh = "sapa.jpg",
                    VanHoa = "Bản sắc văn hóa đặc sắc người H'Mông, Dao Đỏ với các phiên chợ Tình cuối tuần.",
                    DoAn = "Thắng cố, Lợn cắp nách, Cá hồi Sapa nướng tiêu, Thịt trâu gác bếp.",
                    NoiCanToi = "Đỉnh Fansipan, Bản Cát Cát, Đèo Ô Quy Hồ, Thung lũng Mường Hoa."
                },
                new DiaDiem
                {
                    TenDiaDiem = "Nha Trang - Vịnh Ngọc Biển Xanh",
                    DiaChi = "TP. Nha Trang, Tỉnh Khánh Hòa",
                    MaDanhMuc = danhMucs[0].MaDanhMuc,
                    Hotline = "0258.3888.555",
                    TrangThai = "HOAT_DONG",
                    HinhAnh = "nhatrang.avif",
                    VanHoa = "Nền văn hóa Chăm pa cổ kính gắn với Tháp Bà Ponagar linh thiêng.",
                    DoAn = "Nem nướng Ninh Hòa, Bún sứa Nha Trang, Bánh căn hải sản.",
                    NoiCanToi = "VinWonders Nha Trang, Hòn Mun, Tháp Bà Ponagar, Viện Hải dương học."
                },
                new DiaDiem
                {
                    TenDiaDiem = "Đảo Ngọc Phú Quốc",
                    DiaChi = "TP. Phú Quốc, Tỉnh Kiên Giang",
                    MaDanhMuc = danhMucs[0].MaDanhMuc,
                    Hotline = "0297.3999.888",
                    TrangThai = "HOAT_DONG",
                    HinhAnh = "phuquoc.jpg",
                    VanHoa = "Văn hóa làng chài, nghề làm nước mắm truyền thống và trồng hồ tiêu đảo ngọc.",
                    DoAn = "Gỏi cá trích, Bún quậy Kiến Xây, Còi biên mai nướng muối ớt.",
                    NoiCanToi = "Grand World, Bãi Sao, Cáp treo Hòn Thơm, Sunset Sanato."
                }
            };
            context.DiaDiems.AddRange(diaDiems);
            context.SaveChanges();

            // 4. Thêm Tour Du Lịch / Dịch Vụ (DL-13)
            var dichVus = new DichVu[]
            {
                new DichVu
                {
                    TenDichVu = "Tour Khám Phá Đà Nẵng - Bà Nà Hills - Cầu Vàng",
                    MaDiaDiem = diaDiems[0].MaDiaDiem,
                    GiaTien = 3500000,
                    GiaKhuyenMai = 2990000,
                    TyLeCoc = 30,
                    HinhAnhPhong = "danang.webp",
                    SoChoToiDa = 30,
                    SoChoConLai = 12,
                    TrangThaiTour = "DANG_BAN",
                    ThoiLuong = "3 ngày 2 đêm",
                    LichTrinh = "Ngày 1: Đón sân bay Đà Nẵng, tham quan Bán đảo Sơn Trà, tắm biển Mỹ Khê.\nNgày 2: Chinh phục đỉnh Bà Nà Hills, check-in Cầu Vàng, Làng Pháp, trải nghiệm cáp treo.\nNgày 3: Khám phá Ngũ Hành Sơn, mua sắm chợ Hàn, tiễn khách sân bay."
                },
                new DichVu
                {
                    TenDichVu = "Tour Du Thuyền 5 Sao Vịnh Hạ Long",
                    MaDiaDiem = diaDiems[1].MaDiaDiem,
                    GiaTien = 2800000,
                    GiaKhuyenMai = 2450000,
                    TyLeCoc = 30,
                    HinhAnhPhong = "halong.jpg",
                    SoChoToiDa = 24,
                    SoChoConLai = 8,
                    TrangThaiTour = "DANG_BAN",
                    ThoiLuong = "2 ngày 1 đêm",
                    LichTrinh = "Ngày 1: Lên du thuyền, thưởng thức tiệc buffet trưa, chèo kayak khám phá Hang Sửng Sốt, ngắm hoàng hôn vịnh.\nNgày 2: Tập Thái Cực Quyền đón bình minh, tham quan Đảo Ti Tốp, trả phòng và về bến cảng."
                },
                new DichVu
                {
                    TenDichVu = "Tour Sapa Chinh Phục Nóc Nhà Đông Dương Fansipan",
                    MaDiaDiem = diaDiems[3].MaDiaDiem,
                    GiaTien = 2200000,
                    GiaKhuyenMai = null,
                    TyLeCoc = 30,
                    HinhAnhPhong = "sapa.jpg",
                    SoChoToiDa = 35,
                    SoChoConLai = 15,
                    TrangThaiTour = "DANG_BAN",
                    ThoiLuong = "2 ngày 1 đêm",
                    LichTrinh = "Ngày 1: Đến thị trấn Sapa, tham quan bản Cát Cát, thưởng thức đồ nướng đêm Sapa.\nNgày 2: Đi cáp treo lên đỉnh Fansipan 3.143m, chụp ảnh cột cờ Tổ quốc, viếng cụm đền chùa tâm linh."
                },
                new DichVu
                {
                    TenDichVu = "Tour Đà Lạt Ngàn Hoa & Săn Mây Cao Nguyên",
                    MaDiaDiem = diaDiems[2].MaDiaDiem,
                    GiaTien = 2500000,
                    GiaKhuyenMai = 2190000,
                    TyLeCoc = 30,
                    HinhAnhPhong = "dalat.webp",
                    SoChoToiDa = 25,
                    SoChoConLai = 5,
                    TrangThaiTour = "DANG_BAN",
                    ThoiLuong = "3 ngày 2 đêm",
                    LichTrinh = "Ngày 1: Check-in Quảng trường Lâm Viên, đồi Robin, Ga Đà Lạt.\nNgày 2: Săn mây đồi chè Cầu Đất, vườn hoa Cẩm Tú Cầu, Thung Lũng Tình Yêu.\nNgày 3: Dạo chợ đêm Đà Lạt, mua đặc sản mứt hoa quả, trả phòng."
                },
                new DichVu
                {
                    TenDichVu = "Tour Lặn Ngắm San Hô Vịnh Nha Trang",
                    MaDiaDiem = diaDiems[4].MaDiaDiem,
                    GiaTien = 950000,
                    GiaKhuyenMai = 790000,
                    TyLeCoc = 50,
                    HinhAnhPhong = "nhatrang.avif",
                    SoChoToiDa = 40,
                    SoChoConLai = 0,
                    TrangThaiTour = "HET_CHO",
                    ThoiLuong = "1 ngày",
                    LichTrinh = "Sáng 8h00: Ca nô đón tại cảng, di chuyển đến Hòn Mun lặn biển với bình dưỡng khí.\nTrưa: Ăn trưa hải sản tại Làng Chài.\nChiều: Tắm biển Bãi Tranh, chơi dù lượn, ca nô kéo."
                },
                new DichVu
                {
                    TenDichVu = "Tour Khám Phá Nam Đảo Phú Quốc & Sunset Sanato",
                    MaDiaDiem = diaDiems[5].MaDiaDiem,
                    GiaTien = 4200000,
                    GiaKhuyenMai = null,
                    TyLeCoc = 30,
                    HinhAnhPhong = "phuquoc.jpg",
                    SoChoToiDa = 20,
                    SoChoConLai = 20,
                    TrangThaiTour = "TAM_NGUNG",
                    ThoiLuong = "4 ngày 3 đêm",
                    LichTrinh = "Tạm ngừng nhận khách trong đợt bảo trì nâng cấp dịch vụ cuối năm."
                }
            };
            context.DichVus.AddRange(dichVus);
            context.SaveChanges();

            // 5. Thêm Đơn Đặt Tour Mẫu (DL-19)
            var datLichs = new DatLich[]
            {
                new DatLich
                {
                    MaDatLich = "BK100001",
                    MaTraCuu = "TOUR-20261020-1001",
                    MaDichVu = dichVus[0].MaDichVu,
                    MaKhachHang = khachHangs[2].MaNguoiDung,
                    HoTen = "Trần Minh Hoàng",
                    Email = "khachhang@gmail.com",
                    SoDienthoai = "0912345678",
                    NgayThamQuan = DateTime.Now.AddDays(7),
                    SoLuongKhach = 2,
                    SoTienCoc = 1794000,
                    TrangThaiDat = "CHO_XAC_NHAN",
                    ThoiGianTao = DateTime.Now.AddHours(-3)
                },
                new DatLich
                {
                    MaDatLich = "BK100002",
                    MaTraCuu = "TOUR-20261020-1002",
                    MaDichVu = dichVus[1].MaDichVu,
                    MaKhachHang = khachHangs[2].MaNguoiDung,
                    HoTen = "Lê Thị Mai",
                    Email = "maile@gmail.com",
                    SoDienthoai = "0934567890",
                    NgayThamQuan = DateTime.Now.AddDays(10),
                    SoLuongKhach = 4,
                    SoTienCoc = 2940000,
                    TrangThaiDat = "CHO_XAC_NHAN",
                    ThoiGianTao = DateTime.Now.AddHours(-1)
                },
                new DatLich
                {
                    MaDatLich = "BK100003",
                    MaTraCuu = "TOUR-20261019-2003",
                    MaDichVu = dichVus[3].MaDichVu,
                    MaKhachHang = khachHangs[2].MaNguoiDung,
                    HoTen = "Phạm Quang Dũng",
                    Email = "dungpq@gmail.com",
                    SoDienthoai = "0987654321",
                    NgayThamQuan = DateTime.Now.AddDays(5),
                    SoLuongKhach = 3,
                    SoTienCoc = 1971000,
                    TrangThaiDat = "DA_XAC_NHAN",
                    ThoiGianTao = DateTime.Now.AddDays(-1),
                    NguoiXuLy = "Bùi Trung Kiên",
                    ThoiGianXuLy = DateTime.Now.AddHours(-12),
                    GhiChuXuLy = "Đã gọi điện xác nhận lịch bay và khách sạn, gửi vé xác nhận."
                },
                new DatLich
                {
                    MaDatLich = "BK100004",
                    MaTraCuu = "TOUR-20261018-3004",
                    MaDichVu = dichVus[2].MaDichVu,
                    MaKhachHang = khachHangs[2].MaNguoiDung,
                    HoTen = "Vũ Đình Hải",
                    Email = "haivd@yahoo.com",
                    SoDienthoai = "0909123987",
                    NgayThamQuan = DateTime.Now.AddDays(3),
                    SoLuongKhach = 6,
                    SoTienCoc = 3960000,
                    TrangThaiDat = "TU_CHOI",
                    ThoiGianTao = DateTime.Now.AddDays(-2),
                    NguoiXuLy = "Bùi Trung Kiên",
                    ThoiGianXuLy = DateTime.Now.AddDays(-1),
                    LyDoTuChoi = "Thời tiết Sapa mưa bão sạt lở nguy hiểm, đã tư vấn khách chuyển ngày hoặc hoàn cọc.",
                    GhiChuXuLy = "Khách đồng ý nhận lại cọc sau 3 ngày làm việc."
                },
                new DatLich
                {
                    MaDatLich = "BK100005",
                    MaTraCuu = "TOUR-20261010-5005",
                    MaDichVu = dichVus[0].MaDichVu,
                    MaKhachHang = khachHangs[2].MaNguoiDung,
                    HoTen = "Nguyễn Văn Hùng",
                    Email = "hungnv@gmail.com",
                    SoDienthoai = "0911223344",
                    NgayThamQuan = DateTime.Now.AddDays(-5),
                    SoLuongKhach = 2,
                    SoTienCoc = 1794000,
                    TrangThaiDat = "HOAN_THANH",
                    ThoiGianTao = DateTime.Now.AddDays(-10),
                    NguoiXuLy = "Bùi Trung Kiên",
                    ThoiGianXuLy = DateTime.Now.AddDays(-8)
                }
            };
            context.DatLichs.AddRange(datLichs);
            context.SaveChanges();

            // 6. Thêm Đánh Giá Khách Hàng (DL-22)
            var danhGias = new DanhGia[]
            {
                new DanhGia
                {
                    MaDatLich = "BK100005",
                    MaKhachHang = khachHangs[2].MaNguoiDung,
                    MaDiaDiem = diaDiems[0].MaDiaDiem,
                    DiemDanhGia = 5,
                    BinhLuan = "Tour Đà Nẵng chất lượng rất tốt! Khách sạn 4 sao sạch sẽ, bác tài xế vui tính nhiệt tình, hướng dẫn viên kiến thức uyên bác.",
                    DaDuyet = 1,
                    NgayDanhGia = DateTime.Now.AddDays(-3),
                    PhanHoiCuaNhanVien = "TravelWeb chân thành cảm ơn quý khách! Rất hân hạnh được đồng hành cùng quý khách trong các chuyến đi tiếp theo."
                },
                new DanhGia
                {
                    MaDatLich = "BK100003",
                    MaKhachHang = khachHangs[2].MaNguoiDung,
                    MaDiaDiem = diaDiems[2].MaDiaDiem,
                    DiemDanhGia = 4,
                    BinhLuan = "Đà Lạt cảnh đẹp, thời tiết se lạnh rất thích. Tuy nhiên buổi sáng đón hơi sớm xíu nhưng bù lại săn được mây rất đẹp.",
                    DaDuyet = 0,
                    NgayDanhGia = DateTime.Now.AddHours(-10)
                },
                new DanhGia
                {
                    MaDatLich = "BK100001",
                    MaKhachHang = khachHangs[2].MaNguoiDung,
                    MaDiaDiem = diaDiems[1].MaDiaDiem,
                    DiemDanhGia = 1,
                    BinhLuan = "Cần bán số điện thoại sim rác giá rẻ liên hệ 0999xxxxxx quảng cáo vay tiền nhanh chóng...",
                    DaDuyet = 0,
                    NgayDanhGia = DateTime.Now.AddHours(-2)
                }
            };
            context.DanhGias.AddRange(danhGias);
            context.SaveChanges();

            // 7. Thêm Liên Hệ Khách Hàng (DL-22)
            var lienHes = new LienHe[]
            {
                new LienHe
                {
                    HoTen = "Hoàng Anh Tuấn",
                    Email = "tuan.ha@fpt.com.vn",
                    SoDienThoai = "0944556677",
                    TieuDe = "Tư vấn tour công ty quy mô 45 người đi Hạ Long tháng 11",
                    NoiDung = "Chào TravelWeb, công ty mình đang lên kế hoạch tổ chức team building cho khoảng 45 cán bộ nhân viên tại Hạ Long cuối tháng 11. Nhờ bạn tư vấn gói tour du thuyền và báo giá xuất hóa đơn VAT giúp mình nhé.",
                    NgayGui = DateTime.Now.AddHours(-5),
                    TrangThai = "CHUA_XU_LY"
                },
                new LienHe
                {
                    HoTen = "Đặng Thùy Linh",
                    Email = "linh.dt@gmail.com",
                    SoDienThoai = "0977889900",
                    TieuDe = "Hỏi về phụ thu trẻ em dưới 5 tuổi",
                    NoiDung = "Gia đình mình có bé 4 tuổi và bé 8 tuổi thì chính sách giá vé và chỗ ngồi trên xe như thế nào ạ?",
                    NgayGui = DateTime.Now.AddDays(-1),
                    TrangThai = "DA_XU_LY",
                    PhanHoi = "Chào chị Linh, bé dưới 5 tuổi được miễn phí giá tour (ngồi chung với bố mẹ), bé từ 5-9 tuổi tính 75% giá tour có suất ăn và ghế riêng ạ.",
                    NguoiXuLy = "Bùi Trung Kiên",
                    NgayXuLy = DateTime.Now.AddHours(-18)
                }
            };
            context.LienHes.AddRange(lienHes);
            context.SaveChanges();

            // 8. Thêm Nhà Cung Cấp Đối Tác (DL-22)
            var nhaCungCaps = new NhaCungCap[]
            {
                new NhaCungCap
                {
                    TenNhaCungCap = "Khách Sạn Mường Thanh Luxury Đà Nẵng",
                    LoaiDichVu = "Khách sạn / Lưu trú",
                    NguoiLienHe = "Nguyễn Văn Đạt (Trưởng phòng Kinh doanh)",
                    SoDienThoai = "0236.3956.789",
                    Email = "sales@danang.muongthanh.vn",
                    DiaChi = "270 Võ Nguyên Giáp, P. Mỹ An, Q. Ngũ Hành Sơn, Đà Nẵng",
                    TrangThai = "DANG_HOP_TAC",
                    GhiChu = "Đối tác chiến lược lưu trú 4-5 sao cho các tour Miền Trung."
                },
                new NhaCungCap
                {
                    TenNhaCungCap = "Công Ty Vận Tải Du Lịch Hoàng Long Express",
                    LoaiDichVu = "Vận chuyển / Xe du lịch",
                    NguoiLienHe = "Bùi Quốc Tuấn",
                    SoDienThoai = "0912.888.999",
                    Email = "dieuhanh@hoanglongtour.com",
                    DiaChi = "Số 15 Giải Phóng, Hai Bà Trưng, Hà Nội",
                    TrangThai = "DANG_HOP_TAC",
                    GhiChu = "Đội xe Universe 45 chỗ và Limousine 16 chỗ đời mới phục vụ tour miền Bắc."
                },
                new NhaCungCap
                {
                    TenNhaCungCap = "Nhà Hàng Ẩm Thực Cung Đình Huế & Miền Trung",
                    LoaiDichVu = "Ẩm thực / Nhà hàng",
                    NguoiLienHe = "Phan Thị Thu Hương",
                    SoDienThoai = "0234.3822.456",
                    Email = "huongpt@cungdinhhue.vn",
                    DiaChi = "38 Nguyễn Huệ, TP. Huế",
                    TrangThai = "DANG_HOP_TAC",
                    GhiChu = "Phục vụ cơm đoàn, thực đơn đặc sản địa phương từ 150k - 300k/suất."
                },
                new NhaCungCap
                {
                    TenNhaCungCap = "Sun World Fansipan Legend",
                    LoaiDichVu = "Vé tham quan / Cáp treo",
                    NguoiLienHe = "Phòng Vé Đại Lý",
                    SoDienThoai = "0214.3818.888",
                    Email = "booking@fansipanlegend.sunworld.vn",
                    DiaChi = "Đường Nguyễn Chí Thanh, TX. Sapa, Lào Cai",
                    TrangThai = "DANG_HOP_TAC",
                    GhiChu = "Cung cấp vé cáp treo và tàu hỏa leo núi Mường Hoa chiết khấu đại lý."
                }
            };
            context.NhaCungCaps.AddRange(nhaCungCaps);
            context.SaveChanges();
        }
    }
}
