using ThiSmartUI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Đăng ký DbContext với DI Container
// Lifecycle: Scoped - Mỗi request tạo một instance mới
builder.Services.AddDbContext<ExamSystemContext>();

// Cấu hình Session để lưu thông tin đăng nhập
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Timeout sau 30 phút không hoạt động
    options.Cookie.HttpOnly = true; // Chống XSS - JavaScript không thể đọc cookie
    options.Cookie.IsEssential = true; // GDPR - Cookie cần thiết cho ứng dụng
});

// Thêm DistributedMemoryCache cho Session (in-memory storage)
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Session middleware - PHẢI đặt sau UseRouting và trước UseAuthorization
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

