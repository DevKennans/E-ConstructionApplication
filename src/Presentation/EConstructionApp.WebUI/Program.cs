using EConstructionApp.WebUI.Extensions.Exceptions;
using EConstructionApp.Persistence;
using EConstructionApp.Infrastructure;
using EConstructionApp.Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EConstructionApp.WebUI.Middleware;

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
        builder.Services.ConfigureApplicationCookie(config =>
        {
            config.LoginPath = new PathString("/Admin/Auth/Login");
            config.LogoutPath = new PathString("/Admin/Auth/Logout");
            config.Cookie = new CookieBuilder
            {
                Name = "E-Construction",
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                SecurePolicy = CookieSecurePolicy.SameAsRequest
            };
            config.SlidingExpiration = true;
            config.ExpireTimeSpan = TimeSpan.FromDays(7);
            config.AccessDeniedPath = new PathString("/Admin/Auth/AccessDenied");
        });

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ClockSkew = TimeSpan.Zero,

                    ValidIssuer = builder.Configuration["Token:Issuer"],
                    ValidAudience = builder.Configuration["Token:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"]!))
                };
            });

        WebApplication app = builder.Build();

        //app.UseMiddleware<JwtRefreshMiddleware>();
        //app.UseMiddleware<JwtCookieToHeaderMiddleware>();
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
        app.UseAuthentication();
        app.UseAuthorization();
       
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