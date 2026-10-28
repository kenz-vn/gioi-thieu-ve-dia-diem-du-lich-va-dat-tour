# 🌍 HỆ THỐNG WEBSITE GIỚI THIỆU ĐỊA ĐIỂM DU LỊCH & ĐẶT TOUR TRỰC TUYẾN

[![CI/CD Pipeline](https://github.com/kenz-vn/gioi-thieu-ve-dia-diem-du-lich-va-dat-tour/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/kenz-vn/gioi-thieu-ve-dia-diem-du-lich-va-dat-tour/actions)
![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![Docker](https://img.shields.io/badge/Docker-Containerized-2496ED?logo=docker)
![SQL Server](https://img.shields.io/badge/Database-SQL%20Server%202022-CC292B?logo=microsoftsqlserver)
![Bootstrap](https://img.shields.io/badge/UI-Bootstrap%205-7952B3?logo=bootstrap)

---

## 📖 1. GIỚI THIỆU ĐỀ TÀI
Dự án được xây dựng nhằm cung cấp một nền tảng website trung gian giới thiệu địa điểm du lịch và đặt tour trực tuyến tập trung. Hệ thống kết nối đơn vị quản lý khu du lịch với khách tham quan, tối ưu hóa quy trình tìm kiếm địa điểm, đặt chỗ, quản lý thanh toán tiền cọc và theo dõi báo cáo doanh thu theo thời gian thực.

---

## 👥 2. THÀNH VIÊN NHÓM THỰC HIỆN (NHÓM 2)

| STT | Họ và Tên | Vai trò chính trong dự án | Phụ trách cụ thể |
| :---: | :--- | :--- | :--- |
| 1 | **Nguyễn Hữu Khải** *(Trưởng nhóm)* | Project Lead & QA Tester | Quản lý tiến độ, thiết kế sơ đồ Use Case, xây dựng 62 kịch bản kiểm thử (Manual Testing) |
| 2 | **Bùi Trung Kiên** | Core Developer & DevOps | Lập trình module nghiệp vụ chính (DL-03, 09, 13, 19, 22), phân quyền, đóng gói Docker & CI/CD |
| 3 | **Nguyễn Hữu Khang** | Database Architect | Thiết kế chuẩn hóa CSDL (3NF), Entity Framework Core Models & tầng dữ liệu |
| 4 | **Nguyễn Hiếu Minh** | Backend Developer | Lập trình module Quản lý Tour, Tìm kiếm & Lọc danh mục, Báo cáo thống kê |
| 5 | **Vũ Đình Mạnh** | Frontend & UX Specialist | Tối ưu trải nghiệm giao diện người dùng, layout responsive và hiệu ứng tương tác |

---

## 🚀 3. CÔNG NGHỆ ÁP DỤNG (TECH STACK)
- **Backend:** ASP.NET Core MVC (.NET 10.0), C#, Entity Framework Core
- **Database:** Microsoft SQL Server 2022
- **Frontend:** HTML5, CSS3, JavaScript, Bootstrap 5, FontAwesome, SweetAlert2
- **Đóng gói & DevOps:**
  - **Docker & Docker Compose:** Đóng gói toàn bộ ứng dụng web và máy chủ CSDL thành container độc lập.
  - **GitHub Actions (CI/CD):** Tự động kiểm tra cú pháp, build mã nguồn và kiểm thử đóng gói sau mỗi lần đẩy code.
  - **Port Forwarding:** Tích hợp công cụ tạo link truy cập online qua Internet phục vụ kiểm thử từ xa.

---

## ✨ 4. CÁC TÍNH NĂNG NỔI BẬT
- **Dành cho Khách hàng:**
  - Khám phá danh sách địa điểm du lịch, xem chi tiết hình ảnh, giá vé, dịch vụ và chính sách.
  - Tìm kiếm và lọc tour theo địa điểm, danh mục và khoảng giá.
  - Đặt tour trực tuyến với cơ chế kiểm soát số chỗ còn lại theo thời gian thực.
  - Tra cứu lịch sử đặt tour, chi tiết mã đơn và theo dõi trạng thái tiền cọc.
  - Đánh giá, để lại bình luận và chấm điểm các khu du lịch đã trải nghiệm.
- **Dành cho Quản trị viên (Admin & Staff):**
  - Hệ thống xác thực và phân quyền đa cấp bậc (`Customer`, `Staff`, `Admin`).
  - Quản lý danh sách tour, cập nhật trạng thái đơn đặt và xử lý hoàn cọc.
  - Dashboard thống kê doanh thu trực quan, biểu đồ tăng trưởng và top tour bán chạy.

---

## 🛠️ 5. HƯỚNG DẪN KHỞI CHẠY HỆ THỐNG

### Cách 1: Khởi chạy 1-Click bằng Docker (Khuyên dùng)
*Yêu cầu: Máy tính đã cài đặt và đang bật Docker Desktop.*

1. Clone kho mã nguồn về máy:
   ```bash
   git clone https://github.com/kenz-vn/gioi-thieu-ve-dia-diem-du-lich-va-dat-tour.git
   cd gioi-thieu-ve-dia-diem-du-lich-va-dat-tour
   ```
2. Nhấp đúp chuột vào file **`Chay_Bang_Docker.bat`**  
   *(File sẽ tự động nạp đường dẫn Docker, tải SQL Server 2022, build ứng dụng và khởi chạy).*
3. Mở trình duyệt và truy cập: **`http://localhost:5230`**
4. Khi muốn tắt hệ thống, nhấp đúp file: **`Dung_Docker.bat`**

---

### Cách 2: Khởi chạy thủ công bằng .NET SDK
*Yêu cầu: Máy tính đã cài đặt .NET SDK 10.0 và SQL Server LocalDB / Express.*

1. Di chuyển vào thư mục dự án:
   ```bash
   cd TravelWeb
   ```
2. Cập nhật chuỗi kết nối CSDL trong `appsettings.json` (nếu cần).
3. Chạy lệnh:
   ```bash
   dotnet restore
   dotnet run
   ```
4. Truy cập website tại: `http://localhost:5230`

---

## 🌐 6. CHIA SẺ TRANG WEB TRUY CẬP TỪ XA
Khi trang web đang chạy trên máy tính, để người khác ở xa có thể truy cập qua Internet:
1. Nhấp đúp file **`Tao_Link_Chia_Se_Online.bat`**.
2. Hệ thống sẽ sinh ra một đường dẫn công khai miễn phí dạng: `https://xxxx.lhr.life`.
3. Gửi đường link này cho thầy cô hoặc bạn bè để truy cập trực tiếp từ xa trên mọi thiết bị.

---

## 📜 7. GIẤY PHÉP & BẢN QUYỀN
Dự án được phát triển phục vụ học phần Công Nghệ Phần Mềm - Nhóm 2.
Mọi mã nguồn và tài liệu liên quan đều thuộc bản quyền của nhóm tác giả.
