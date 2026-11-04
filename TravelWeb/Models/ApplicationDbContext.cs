using Microsoft.EntityFrameworkCore;

namespace TravelWeb.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<DanhMuc> DanhMucs { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<DiaDiem> DiaDiems { get; set; }
        public DbSet<DichVu> DichVus { get; set; }
        public DbSet<DatLich> DatLichs { get; set; }
        public DbSet<ThanhToan> ThanhToans { get; set; }
        public DbSet<DanhGia> DanhGias { get; set; }
        public DbSet<LienHe> LienHes { get; set; }
        public DbSet<NhaCungCap> NhaCungCaps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình DAT_LICH
            modelBuilder.Entity<DatLich>(entity =>
            {
                entity.HasIndex(e => e.MaDatLich).IsUnique();
                entity.HasIndex(e => e.MaTraCuu).IsUnique();

                entity.HasOne(e => e.KhachHang)
                    .WithMany(k => k.DatLichs)
                    .HasForeignKey(e => e.MaKhachHang)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.DichVu)
                    .WithMany(d => d.DatLichs)
                    .HasForeignKey(e => e.MaDichVu)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Cấu hình THANH_TOAN
            modelBuilder.Entity<ThanhToan>(entity =>
            {
                entity.HasOne(e => e.DatLich)
                    .WithMany(d => d.ThanhToans)
                    .HasPrincipalKey(d => d.MaDatLich)
                    .HasForeignKey(e => e.MaDatLich)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Cấu hình DANH_GIA (Khắc phục lỗi Multiple Cascade Paths trong SQL Server)
            modelBuilder.Entity<DanhGia>(entity =>
            {
                entity.HasOne(e => e.DatLich)
                    .WithMany(d => d.DanhGias)
                    .HasPrincipalKey(d => d.MaDatLich)
                    .HasForeignKey(e => e.MaDatLich)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.DiaDiem)
                    .WithMany()
                    .HasForeignKey(e => e.MaDiaDiem)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.KhachHang)
                    .WithMany()
                    .HasForeignKey(e => e.MaKhachHang)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Cấu hình DIA_DIEM
            modelBuilder.Entity<DiaDiem>(entity =>
            {
                entity.HasOne(e => e.ChuSoHuu)
                    .WithMany()
                    .HasForeignKey(e => e.MaChuSoHuu)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.DanhMuc)
                    .WithMany(dm => dm.DiaDiems)
                    .HasForeignKey(e => e.MaDanhMuc)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
