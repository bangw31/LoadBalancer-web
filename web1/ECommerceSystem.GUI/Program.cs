using ECommerceSystem.GUI.Apis;
using ECommerceSystem.GUI.Services;
using ECommerceSystem.Shared.Constants;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Refit;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add MVC + HttpContextAccessor
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Custom services
builder.Services.AddScoped<AuthService>();
builder.Services.AddTransient<AuthRetryHandler>();

// Authentication (cookie for GUI)
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Home/Error";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
    });

// Register Refit clients. Do NOT attempt to read HttpContext here.
// Rely on AuthRetryHandler to attach Authorization per outgoing request.
static void ConfigureRefit(IServiceCollection services)
{
    void ConfigureClient(HttpClient client)
    {
        client.BaseAddress = new Uri(AppConstants.ApiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(20);
    }

    services.AddRefitClient<IProductApi>()
        .AddHttpMessageHandler<AuthRetryHandler>()
        .ConfigureHttpClient((sp, client) => ConfigureClient(client));

    services.AddRefitClient<IAuthApi>()
        .AddHttpMessageHandler<AuthRetryHandler>()
        .ConfigureHttpClient((sp, client) => ConfigureClient(client));

    services.AddRefitClient<ICategoryApi>()
        .AddHttpMessageHandler<AuthRetryHandler>()
        .ConfigureHttpClient((sp, client) => ConfigureClient(client));

    services.AddRefitClient<IOrderApi>()
        .AddHttpMessageHandler<AuthRetryHandler>()
        .ConfigureHttpClient((sp, client) => ConfigureClient(client));

    services.AddRefitClient<IUserApi>()
        .AddHttpMessageHandler<AuthRetryHandler>()
        .ConfigureHttpClient((sp, client) => ConfigureClient(client));

    services.AddRefitClient<ICartApi>()
        .AddHttpMessageHandler<AuthRetryHandler>()
        .ConfigureHttpClient((sp, client) => ConfigureClient(client));

    services.AddRefitClient<IAdminApi>()
        .AddHttpMessageHandler<AuthRetryHandler>()
        .ConfigureHttpClient((sp, client) => ConfigureClient(client));
}

ConfigureRefit(builder.Services);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();