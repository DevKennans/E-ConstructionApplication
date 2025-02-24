using EConstructionApp.WebUI.Extensions.Exceptions;
using EConstructionApp.Persistence;
using EConstructionApp.Infrastructure;
using EConstructionApp.Application;
internal class Program
{
    private static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddApplication();
        builder.Services.AddInfrastructure();
        builder.Services.AddPersistence(builder.Configuration);
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Admin/Login";
            options.AccessDeniedPath = "/Home/ServerError";
        });
  
        WebApplication app = builder.Build();
        app.UseMiddleware<GlobalErrorHandlerMiddleware>();
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error/ServerError");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthorization();
        app.MapControllerRoute(
            name: "admin_default",
            pattern: "Admin",
            defaults: new { area = "Admin", controller = "Auth", action = "Login" });

        app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Auth}/{action=Login}/{id?}",
            defaults: new { area = "Admin" });


        app.Run();
    }
}