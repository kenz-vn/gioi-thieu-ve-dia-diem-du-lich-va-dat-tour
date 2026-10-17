using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DuLichWeb.Services
{
    public class BookingDTO
    {
        public int Id { get; set; }
        public string MaDatLich { get; set; }
        public string MaTraCuu { get; set; }
        public string TenDichVu { get; set; }
        public string TenDiaDiem { get; set; }
        public DateTime NgayThamQuan { get; set; }
        public int SoLuongKhach { get; set; }
        public decimal SoTienCoc { get; set; }
        public string TrangThaiDat { get; set; }
        public DateTime ThoiGianTao { get; set; }
    }

    public class BookingService
    {
        private readonly string _connectionString;

        public BookingService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // 1. Lưu trữ đơn đặt tour & tự động sinh mã đặt lịch, mã tra cứu
        public bool TaoDonDatTour(int maKhachHang, int maDichVu, DateTime ngayThamQuan, int soLuongKhach, decimal giaTien, decimal tyLeCoc, out string maDatLich, out string maTraCuu)
        {
            maDatLich = "BK-" + DateTime.Now.ToString("yyyyMMddHHmmss");
            maTraCuu = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
            decimal soTienCoc = (giaTien * soLuongKhach * tyLeCoc) / 100m;

            string sql = @"
                INSERT INTO DAT_LICH (ma_dat_lich, ma_khach_hang, ma_dich_vu, ma_tra_cuu, ngay_tham_quan, so_luong_khach, so_tien_coc, trang_thai_dat, thoi_gian_tao)
                VALUES (@ma_dat_lich, @ma_khach_hang, @ma_dich_vu, @ma_tra_cuu, @ngay_tham_quan, @so_luong_khach, @so_tien_coc, N'CHỜ XÁC NHẬN', GETDATE())";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma_dat_lich", maDatLich);
                cmd.Parameters.AddWithValue("@ma_khach_hang", maKhachHang);
                cmd.Parameters.AddWithValue("@ma_dich_vu", maDichVu);
                cmd.Parameters.AddWithValue("@ma_tra_cuu", maTraCuu);
                cmd.Parameters.AddWithValue("@ngay_tham_quan", ngayThamQuan);
                cmd.Parameters.AddWithValue("@so_luong_khach", soLuongKhach);
                cmd.Parameters.AddWithValue("@so_tien_coc", soTienCoc);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }

        // 2. Truy vấn lịch sử đặt tour theo mã khách hàng
        public List<BookingDTO> LayLichSuDatTour(int maKhachHang)
        {
            var result = new List<BookingDTO>();

            string sql = @"
                SELECT dl.id, dl.ma_dat_lich, dl.ma_tra_cuu, dv.ten_dich_vu, dd.ten_dia_diem, 
                       dl.ngay_tham_quan, dl.so_luong_khach, dl.so_tien_coc, dl.trang_thai_dat, dl.thoi_gian_tao
                FROM DAT_LICH dl
                JOIN DICH_VU dv ON dl.ma_dich_vu = dv.ma_dich_vu
                JOIN DIA_DIEM dd ON dv.ma_dia_diem = dd.ma_dia_diem
                WHERE dl.ma_khach_hang = @MaKhachHang
                ORDER BY dl.thoi_gian_tao DESC";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaKhachHang", maKhachHang);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new BookingDTO
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            MaDatLich = reader["ma_dat_lich"].ToString(),
                            MaTraCuu = reader["ma_tra_cuu"].ToString(),
                            TenDichVu = reader["ten_dich_vu"].ToString(),
                            TenDiaDiem = reader["ten_dia_diem"].ToString(),
                            NgayThamQuan = Convert.ToDateTime(reader["ngay_tham_quan"]),
                            SoLuongKhach = Convert.ToInt32(reader["so_luong_khach"]),
                            SoTienCoc = Convert.ToDecimal(reader["so_tien_coc"]),
                            TrangThaiDat = reader["trang_thai_dat"].ToString(),
                            ThoiGianTao = Convert.ToDateTime(reader["thoi_gian_tao"])
                        });
                    }
                }
            }
            return result;
        }
    }
}