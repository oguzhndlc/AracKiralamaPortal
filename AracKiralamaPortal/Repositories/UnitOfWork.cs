using AracKiralamaPortal.Data;
using AracKiralamaPortal.Models;

namespace AracKiralamaPortal.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IRepository<Brand> Brands { get; private set; }
        public IRepository<Car> Cars { get; private set; }
        public IRepository<VehicleType> VehicleTypes { get; private set; }
        public IRepository<VehicleSubType> VehicleSubTypes { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            Brands = new Repository<Brand>(context);
            Cars = new Repository<Car>(context);
            VehicleTypes = new Repository<VehicleType>(context);
            VehicleSubTypes = new Repository<VehicleSubType>(context);

        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
