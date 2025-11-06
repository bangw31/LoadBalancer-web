var builder = WebApplication.CreateBuilder(args);

// ✅ Đăng ký dịch vụ trước khi build
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient(); // <-- Fix ở đây

var app = builder.Build();

// 🔒 Middleware kiểm tra xác thực và quyền truy cập
app.Use(async (context, next) =>
{
    var path = context.Request.Path;

    if (context.User.Identity?.IsAuthenticated == true)
    {
        if (context.User.IsInRole("Admin"))
        {
            // Nếu đã ở admin, tiếp tục
            if (path.StartsWithSegments("/admin"))
            {
                await next();
            }
            else
            {
                context.Response.Redirect("/admin/dashboard");
            }
        }
        else
        {
            // Người dùng thường, nếu chưa ở /products thì redirect
            if (!path.StartsWithSegments("/products"))
            {
                context.Response.Redirect("/products");
            }
            else
            {
                await next();
            }
        }
    }
    else
    {
        // Chưa đăng nhập, nếu chưa ở /products thì redirect
        if (!path.StartsWithSegments("/products"))
        {
            context.Response.Redirect("/products");
        }
        else
        {
            await next();
        }
    }
});


// 🔧 Middleware mặc định
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Cần để phục vụ static assets (nếu có)
app.UseRouting();
app.UseAuthorization();

// 🔀 Định tuyến
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
