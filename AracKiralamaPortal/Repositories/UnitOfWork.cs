using AracKiralamaPortal.Data;
using AracKiralamaPortal.Models;

namespace AracKiralamaPortal.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IRepository<Brand> Brands { get; private set; }
        public IRepository<Car> Cars { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            Brands = new Repository<Brand>(context);
            Cars = new Repository<Car>(context);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
