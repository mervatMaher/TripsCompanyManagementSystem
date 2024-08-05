using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TripsCompanySystem.Data;
using TripsCompanySystem.Models;

namespace TripsCompanySystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            var builder = WebApplication.CreateBuilder(args);
            var Config = builder.Configuration;

            var options = new DbContextOptionsBuilder<TripsCompanySystemDbContext>()
                .UseSqlServer(Config.GetConnectionString("DefaultConnection"))
                .Options;

            builder.Services.AddDbContext<TripsCompanySystemDbContext>(options =>
            options.UseSqlServer(Config.GetConnectionString("DefaultConnection")));


            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false).
                AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<TripsCompanySystemDbContext>()
                .AddDefaultTokenProviders();

            TripsCompanySystemDbContext dbContext = new TripsCompanySystemDbContext(options);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.AllowAnyOrigin()
                                         .AllowAnyHeader()
                                         .AllowAnyMethod();

                                  });
            });


            // Add services to the container.
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
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
