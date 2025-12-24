using AracKiralamaPortal.Models;

namespace AracKiralamaPortal.Repositories
{
    public interface IUnitOfWork
    {
        IRepository<Brand> Brands { get; }
        IRepository<Car> Cars { get; }
        IRepository<VehicleType> VehicleTypes { get; }
        IRepository<VehicleSubType> VehicleSubTypes { get; }
        void Save();
    }
}
