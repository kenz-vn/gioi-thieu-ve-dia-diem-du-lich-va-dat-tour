var builder = WebApplication.CreateBuilder(args);

// Thêm dòng này để các Service có thể lấy được chuỗi kết nối từ appsettings.json
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Đăng ký các Service bạn vừa tạo vào hệ thống (để dùng trong Controller)
builder.Services.AddTransient<DuLichWeb.Services.TourService>();
builder.Services.AddTransient<DuLichWeb.Services.BookingService>();
builder.Services.AddTransient<DuLichWeb.Services.ReportService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
