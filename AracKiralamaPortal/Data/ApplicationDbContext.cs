using AracKiralamaPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace AracKiralamaPortal.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Brand> Brands { get; set; }
        public DbSet<Car> Cars { get; set; }
    }
}
