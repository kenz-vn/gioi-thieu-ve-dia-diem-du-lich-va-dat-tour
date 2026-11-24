# BÁO CÁO PHÂN CÔNG NHIỆM VỤ & ĐÓNG GÓP MÃ NGUỒN

## Đề tài: Xây dựng Website Giới thiệu Khu Du lịch và Đặt Tour Trực tuyến (TravelWeb)
* **Thành viên:** Nguyễn Hữu Khải (Nguyễn Văn Khải)
* **Tài khoản GitHub:** [nguyenhuukhai2807-glitch](https://github.com/nguyenhuukhai2807-glitch)
* **Email:** nguyenhuukhai2807@gmail.com
* **Nhánh Git phụ trách:** `feature/khai` (khai-frontend)
* **Vai trò trong nhóm:** Frontend Lead & Phân hệ Khách hàng (Tour & Đặt tour)

---

## 1. Danh sách User Stories đảm nhận (Product Backlog)

| Mã US | User Story & Mô tả nghiệp vụ | Sprint | Story Points | Kết quả |
| :--- | :--- | :---: | :---: | :---: |
| **US-07** | Xem danh sách địa điểm du lịch, banner slide giới thiệu KDL | Sprint 1 | 3 SP | Hoàn thành |
| **US-14** | Tìm kiếm và lọc tour theo danh mục, từ khóa và khoảng giá | Sprint 2 | 5 SP | Hoàn thành |
| **US-16** | Xem chi tiết tour, timeline lịch trình đa chặng và khách sạn | Sprint 2 | 5 SP | Hoàn thành |
| **US-18** | Đặt tour trực tuyến, tự động tính tổng tiền và tiền cọc (30% - 50%) | Sprint 3 | 8 SP | Hoàn thành |
| **US-19** | Tự sinh mã tra cứu công khai (`TOUR-YYYYMMDD-XXXX`) và tạm giữ số chỗ | Sprint 3 | 5 SP | Hoàn thành |
| **US-03** | Tra cứu tiến độ đơn qua mã công khai và xem lịch sử chuyến đi cá nhân | Sprint 3 | 5 SP | Hoàn thành |
| **US-20** | Khách hàng gửi yêu cầu hủy đơn và tự động hoàn trả số chỗ về kho tour | Sprint 3 | 5 SP | Hoàn thành |

---

## 2. Chi tiết các tệp mã nguồn và thư mục phụ trách

### A. Controllers (Tầng điều khiển nghiệp vụ)
1. **`TravelWeb/Controllers/HomeController.cs`**
   - Xử lý Action `Index`: tìm kiếm theo từ khóa tên tour/địa điểm, lọc theo danh mục địa điểm, phân trang (6 tour/trang).
   - Xử lý Action `LienHe` (GET/POST): tiếp nhận biểu mẫu liên hệ, lưu trữ thắc mắc của khách hàng vào CSDL với trạng thái `CHUA_XU_LY`.
2. **`TravelWeb/Controllers/TourController.cs`**
   - Xử lý Action `Index`: danh mục tour công khai, tìm kiếm và lọc đa tiêu chí.
   - Xử lý Action `Details`: điều hướng người dùng sang trang chi tiết và đặt chỗ.
3. **`TravelWeb/Controllers/BookingController.cs`**
   - Action `DatTour`: kiểm tra session đăng nhập, nạp thông tin tour và khách hàng.
   - Action `XacNhanDatTour`: kiểm tra ngày đi (>= hôm nay), số khách hợp lệ, kiểm soát số chỗ chống âm kho, tự động tính tiền vé người lớn / trẻ em (60%), tính tiền cọc (30% - 50%), sinh mã tra cứu công khai `TOUR-YYYYMMDD-XXXX` và tạm giữ số chỗ.
   - Action `LichSuDatTour`: tra cứu đơn công khai bằng `maTraCuu` hoặc xem lịch sử tất cả các đơn đặt của tài khoản đang đăng nhập.
   - Action `ChiTietDon`: hiển thị đầy đủ thông tin vé điện tử, khách sạn, loại phòng và số tiền cọc.
   - Action `HuyDon`: kiểm tra điều kiện hủy (trước ngày đi >= 2 ngày), cập nhật trạng thái `DA_HUY` và tự động cộng hoàn trả lại số chỗ cho tour.

### B. Views (Tầng giao diện người dùng)
1. **`TravelWeb/Views/Home/Index.cshtml`**: Giao diện trang chủ, banner giới thiệu, lối tắt điều hướng, danh sách tour.
2. **`TravelWeb/Views/Home/LienHe.cshtml`**: Giao diện gửi liên hệ tư vấn, thông tin hotline và trụ sở hỗ trợ.
3. **`TravelWeb/Views/Tour/Index.cshtml`**: Lưới thẻ tour Responsive Bootstrap 5, bộ lọc từ khóa, danh mục và phân trang.
4. **`TravelWeb/Views/Booking/DatTour.cshtml`**: Form đặt tour, bảng giá vé, JavaScript tự động tính tổng tiền và tiền cọc theo số lượng khách.
5. **`TravelWeb/Views/Booking/LichSuDatTour.cshtml`**: Bảng lịch sử đơn hàng, huy hiệu màu trạng thái và ô tra cứu nhanh theo mã.
6. **`TravelWeb/Views/Booking/ChiTietDon.cshtml`**: Vé du lịch điện tử chi tiết, thông tin khách sạn lưu trú và nút hủy đơn có điều kiện.

### C. Stylesheet (CSS giao diện)
1. **`TravelWeb/wwwroot/css/site.css`**: Tùy biến thanh cuộn mượt mà (smooth scroll), thanh cuộn webkit bo góc màu xanh thương hiệu, định dạng form floating và độ tương phản giao diện.

---

## 3. Kết quả kiểm thử chức năng (Test Execution)

| Mã TC | Nội dung ca kiểm thử | Kết quả | Ghi chú |
| :--- | :--- | :---: | :--- |
| **TC-01** | Tìm kiếm tour theo từ khóa tên tour / địa điểm | **PASS** | Kết quả lọc chính xác theo từ khóa |
| **TC-02** | Lọc tour theo danh mục địa điểm | **PASS** | Hiển thị đúng danh mục được chọn |
| **BK01** | Đặt tour hợp lệ khi còn đủ chỗ trống | **PASS** | Đơn được tạo với mã TOUR-YYYYMMDD-XXXX |
| **BK02** | Đặt tour với ngày đi trong quá khứ | **PASS** | Báo lỗi và chặn đặt ngày quá khứ |
| **BK03** | Đặt số khách vượt quá số chỗ trống còn lại | **PASS** | Chặn submit khi vượt quá sức chứa |
| **BK04** | Tự động tính tiền cọc và giá vé trẻ em | **PASS** | Đúng 100% theo tỷ lệ cọc cấu hình |
| **BK05** | Hủy đơn đặt tour và hoàn trả lại số chỗ | **PASS** | Số chỗ của tour tự động tăng hoàn lại |
