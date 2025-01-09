using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using Webapp.Models.Movies;
using Webapp.Models.Olympics;
using Microsoft.AspNetCore.Identity;

namespace Webapp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddDbContext<MoviesDbContext>(op =>
        {
            op.UseSqlite(builder.Configuration["MoviesDatabase:ConnectionString"]);
        });

        // builder.Services.AddDefaultIdentity<IdentityUser>(
        //     options => options.SignIn.RequireConfirmedAccount = true
        //     
        // ).AddEntityFrameworkStores<AppDbContext>();
        builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 5;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();
        
        
        builder.Services.AddDbContext<OlympicsContext>(options =>
        {
            options.UseSqlite(builder.Configuration["OlympicsDatabase:ConnectionString"]);
        });
        
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(builder.Configuration["AccountDatabase:ConnectionString"]);
        });
        
        // logowanie
        builder.Services.AddRazorPages();                         // dodać
       
        builder.Services.AddMemoryCache();                        // dodać
        builder.Services.AddSession();                            // dodać
        
        var app = builder.Build();
        app.UseAuthentication();                                 // dodać
        app.UseAuthorization();                                  // dodać
        app.UseSession();                                        // dodać 
        app.MapRazorPages();                                     // dodać   
    
            
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

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{PersonId?}");

        app.Run();
    }
}