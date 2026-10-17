using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DuLichWeb.Services
{
    public class BaoCaoTongQuanModel
    {
        public int TongSoDon { get; set; }
        public int TongKhach { get; set; }
        public decimal TongDoanhThuCoc { get; set; }
    }

    public class TourPhoBienModel
    {
        public int MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public int SoLuotDat { get; set; }
        public int TongKhachDat { get; set; }
    }

    public class ReportService
    {
        private readonly string _connectionString;

        public ReportService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // 1. Thống kê tổng số đơn, tổng khách và tổng doanh thu tiền cọc theo khoảng thời gian
        public BaoCaoTongQuanModel LayThongKeTongQuan(DateTime tuNgay, DateTime denNgay)
        {
            var model = new BaoCaoTongQuanModel();
            string sql = @"
                SELECT 
                    COUNT(id) AS TongSoDon,
                    ISNULL(SUM(so_luong_khach), 0) AS TongKhach,
                    ISNULL(SUM(so_tien_coc), 0) AS TongDoanhThuCoc
                FROM DAT_LICH
                WHERE trang_thai_dat != N'ĐÃ HỦY'
                  AND CAST(thoi_gian_tao AS DATE) BETWEEN @TuNgay AND @DenNgay";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay.Date);
                cmd.Parameters.AddWithValue("@DenNgay", denNgay.Date);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model.TongSoDon = Convert.ToInt32(reader["TongSoDon"]);
                        model.TongKhach = Convert.ToInt32(reader["TongKhach"]);
                        model.TongDoanhThuCoc = Convert.ToDecimal(reader["TongDoanhThuCoc"]);
                    }
                }
            }
            return model;
        }

        // 2. Thống kê Top các tour được đặt phổ biến nhất
        public List<TourPhoBienModel> LayTopTourPhoBien(int topLimit = 5)
        {
            var result = new List<TourPhoBienModel>();
            string sql = $@"
                SELECT TOP {topLimit}
                    dv.ma_dich_vu,
                    dv.ten_dich_vu,
                    COUNT(dl.id) AS SoLuotDat,
                    ISNULL(SUM(dl.so_luong_khach), 0) AS TongKhachDat
                FROM DAT_LICH dl
                JOIN DICH_VU dv ON dl.ma_dich_vu = dv.ma_dich_vu
                WHERE dl.trang_thai_dat != N'ĐÃ HỦY'
                GROUP BY dv.ma_dich_vu, dv.ten_dich_vu
                ORDER BY SoLuotDat DESC";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new TourPhoBienModel
                        {
                            MaDichVu = Convert.ToInt32(reader["ma_dich_vu"]),
                            TenDichVu = reader["ten_dich_vu"].ToString(),
                            SoLuotDat = Convert.ToInt32(reader["SoLuotDat"]),
                            TongKhachDat = Convert.ToInt32(reader["TongKhachDat"])
                        });
                    }
                }
            }
            return result;
        }
    }
}