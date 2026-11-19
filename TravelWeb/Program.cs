using Microsoft.EntityFrameworkCore;
using TravelWeb.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Cấu hình Session để phục vụ phân quyền và đăng nhập
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Đăng ký HttpContextAccessor để Views có thể truy cập Session trực tiếp
builder.Services.AddHttpContextAccessor();

// Đăng ký kết nối CSDL với SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Khởi tạo Database và Seed Data tự động khi khởi động (hỗ trợ tự động thử lại khi SQL Server Container đang khởi động)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    const int maxRetries = 10;
    var delay = TimeSpan.FromSeconds(3);

    for (int retry = 1; retry <= maxRetries; retry++)
    {
        try
        {
            logger.LogInformation("Đang kiểm tra kết nối CSDL (lần thử {Retry}/{MaxRetries})...", retry, maxRetries);
            var context = services.GetRequiredService<ApplicationDbContext>();
            DbInitializer.Initialize(context);
            logger.LogInformation("Khởi tạo và nạp dữ liệu mẫu CSDL thành công!");
            break;
        }
        catch (Exception ex)
        {
            if (retry == maxRetries)
            {
                logger.LogError(ex, "Lỗi xảy ra trong quá trình khởi tạo CSDL sau {MaxRetries} lần thử: {Message}", maxRetries, ex.Message);
            }
            else
            {
                logger.LogWarning("CSDL chưa sẵn sàng ({Message}). Đang thử lại sau {Delay} giây...", ex.Message, delay.TotalSeconds);
                System.Threading.Thread.Sleep(delay);
            }
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseRouting();

// Sử dụng Session trước Authorization
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
