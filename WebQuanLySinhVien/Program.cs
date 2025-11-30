using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// ==============================================
// 1. ĐĂNG KÝ DỊCH VỤ (SERVICES)
// ==============================================

// A. Cấu hình Session & Cache (Bắt buộc cho ứng dụng phân tán)
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    // Đặt thời gian timeout là 30 phút (đủ lâu để thuyết trình)
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    // Đảm bảo Session hoạt động ngay cả khi chưa đồng ý Cookie Policy
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// B. Đăng ký HttpContextAccessor (SỬA LỖI LAYOUT)
// Dòng này cực kỳ quan trọng để file _Layout.cshtml lấy được thông tin người dùng đang đăng nhập
builder.Services.AddHttpContextAccessor();

// C. Đăng ký MVC (Controller + View)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ==============================================
// 2. CẤU HÌNH PIPELINE (MIDDLEWARE)
// ==============================================

// Cấu hình xử lý lỗi cho môi trường Production/Development
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Cho phép load CSS, JS, Ảnh

app.UseRouting();

// [QUAN TRỌNG] Bật Session trước khi kiểm tra quyền hạn (Authorization)
app.UseSession();

app.UseAuthorization();

// Định tuyến mặc định: Vào Home -> Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();