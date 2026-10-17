using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DuLichWeb.Services
{
    public class TourDTO
    {
        public int MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public decimal GiaTien { get; set; }
        public decimal TyLeCoc { get; set; }
        public string HinhAnh { get; set; }
        public string TenDiaDiem { get; set; }
        public string TenDanhMuc { get; set; }
    }

    public class TourService
    {
        private readonly string _connectionString;

        // Tự động lấy chuỗi kết nối từ appsettings.json thông qua DI (Dependency Injection)
        public TourService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<TourDTO> LayDanhSachTour(string keyword, int? maDanhMuc, decimal? minGia, decimal? maxGia, int page = 1, int pageSize = 10)
        {
            var result = new List<TourDTO>();

            string sql = @"
                SELECT dv.ma_dich_vu, dv.ten_dich_vu, dv.gia_tien, dv.ty_le_coc, dv.hinh_anh_phong, dd.ten_dia_diem, dm.ten_danh_muc
                FROM DICH_VU dv
                JOIN DIA_DIEM dd ON dv.ma_dia_diem = dd.ma_dia_diem
                JOIN DANH_MUC dm ON dd.ma_danh_muc = dm.ma_danh_muc
                WHERE (@Keyword IS NULL OR dv.ten_dich_vu LIKE '%' + @Keyword + '%' OR dd.ten_dia_diem LIKE '%' + @Keyword + '%')
                  AND (@MaDanhMuc IS NULL OR dd.ma_danh_muc = @MaDanhMuc)
                  AND (@MinGia IS NULL OR dv.gia_tien >= @MinGia)
                  AND (@MaxGia IS NULL OR dv.gia_tien <= @MaxGia)
                ORDER BY dv.ma_dich_vu DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Keyword", (object)keyword ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MaDanhMuc", (object)maDanhMuc ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MinGia", (object)minGia ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MaxGia", (object)maxGia ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new TourDTO
                        {
                            MaDichVu = Convert.ToInt32(reader["ma_dich_vu"]),
                            TenDichVu = reader["ten_dich_vu"].ToString(),
                            GiaTien = Convert.ToDecimal(reader["gia_tien"]),
                            TyLeCoc = Convert.ToDecimal(reader["ty_le_coc"]),
                            HinhAnh = reader["hinh_anh_phong"]?.ToString(),
                            TenDiaDiem = reader["ten_dia_diem"].ToString(),
                            TenDanhMuc = reader["ten_danh_muc"].ToString()
                        });
                    }
                }
            }
            return result;
        }
    }
}