using AnyPasteSite.Data;
using Microsoft.EntityFrameworkCore;

namespace AnyPasteSite
{
    public static class Program
    {
        public static void Main()
        {
            var builder = WebApplication.CreateBuilder();

            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IRepository, Repository>();

            if (builder.Environment.IsDevelopment())
            {
                Console.WriteLine("--> using InMem Db");

                builder.Services.AddDbContext<AppDbContext>(opt =>
                {
                    opt.UseInMemoryDatabase("InMem");
                }, ServiceLifetime.Scoped);
            }
            else
            {
                Console.WriteLine("--> using SqlLite Db");

                builder.Services.AddDbContext<AppDbContext>(opt =>
                {
                    opt.UseSqlite("AnyPaste.db");
                }, ServiceLifetime.Scoped);
            }

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseStatusCodePages();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

