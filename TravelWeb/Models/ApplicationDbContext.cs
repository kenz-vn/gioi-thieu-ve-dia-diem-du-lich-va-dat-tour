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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DatLich>(entity =>
            {
                entity.HasIndex(e => e.MaDatLich).IsUnique();
                entity.HasIndex(e => e.MaTraCuu).IsUnique();
            });

            modelBuilder.Entity<ThanhToan>(entity =>
            {
                entity.HasOne(e => e.DatLich)
                    .WithMany(d => d.ThanhToans)
                    .HasPrincipalKey(d => d.MaDatLich)
                    .HasForeignKey(e => e.MaDatLich);
            });

            modelBuilder.Entity<DanhGia>(entity =>
            {
                entity.HasOne(e => e.DatLich)
                    .WithMany(d => d.DanhGias)
                    .HasPrincipalKey(d => d.MaDatLich)
                    .HasForeignKey(e => e.MaDatLich);
            });
        }
    }
}
