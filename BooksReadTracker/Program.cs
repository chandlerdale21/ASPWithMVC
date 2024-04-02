using BooksReadTracker.Data;
using BooksReadTrackerDatabaseLayer;
using BooksReadTrackerDBLibrary;
using BooksReadTrackerServiceLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BooksReadTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            var BooksReadDbConnection = builder.Configuration.GetConnectionString("BooksReadDbConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<BooksReadTrackerDbContext>(options =>
                options.UseSqlServer(BooksReadDbConnection));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();


            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();


            //add our own services:
            builder.Services.AddScoped<IBooksRepository, BooksRepository>();
            builder.Services.AddScoped<ICategoriesRepository, CategoryRepository>();

            builder.Services.AddScoped<IBooksServices, BooksService>();
            builder.Services.AddScoped<ICategoriesService, CategoriesService>();


            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(connectionString).Options;
            using (var context = new ApplicationDbContext(contextOptions))
            {
                context.Database.Migrate();
            }
            var contextOptions2 = new DbContextOptionsBuilder<BooksReadTrackerDbContext>().UseSqlServer(BooksReadDbConnection).Options;
            using (var context = new BooksReadTrackerDbContext(contextOptions2))
            {
                context.Database.Migrate();
            }

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
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
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
