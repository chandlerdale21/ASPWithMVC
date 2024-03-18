using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;



namespace BooksReadTrackerDBLibrary
{
    public class BooksReadTrackerDbContext : DbContext
    {
        private static IConfigurationRoot _configuration;
        public BooksReadTrackerDbContext()
        {

        }
        public BooksReadTrackerDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                _configuration = builder.Build();
                var cnstr = _configuration.GetConnectionString("BooksReadDbConnection");
                optionsBuilder.UseSqlServer(cnstr);
            }
        }

    }
}
