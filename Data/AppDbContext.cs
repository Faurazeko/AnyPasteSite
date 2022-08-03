using AnyPasteSite.Models;
using Microsoft.EntityFrameworkCore;

namespace AnyPasteSite.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<UploadInfo> UploadInfo { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {
            Database.EnsureCreated();
        }
    }
}
