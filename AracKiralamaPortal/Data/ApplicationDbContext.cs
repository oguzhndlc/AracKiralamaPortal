using AracKiralamaPortal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AracKiralamaPortal.Data   
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }    
        public DbSet<Brand> Brands { get; set; }
        public DbSet<VehicleType> VehicleTypes { get; set; }
        public DbSet<VehicleSubType> VehicleSubTypes { get; set; }
        public DbSet<Reservation> Reservations { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<VehicleType>().HasData(
                new VehicleType { Id = 1, Name = "Otomobil" },
                new VehicleType { Id = 2, Name = "Motosiklet" },
                new VehicleType { Id = 3, Name = "ATV / Arazi Aracı" },
                new VehicleType { Id = 4, Name = "Minivan" },
                new VehicleType { Id = 5, Name = "Elektrikli Araç" }
            );

            modelBuilder.Entity<VehicleSubType>().HasData(
                // Otomobil
                new VehicleSubType { Id = 1, Name = "Sedan", VehicleTypeId = 1 },
                new VehicleSubType { Id = 2, Name = "SUV", VehicleTypeId = 1 },
                new VehicleSubType { Id = 3, Name = "Hatchback", VehicleTypeId = 1 },
                new VehicleSubType { Id = 4, Name = "Coupe", VehicleTypeId = 1 },
                new VehicleSubType { Id = 5, Name = "Cabrio / Üstü Açılır", VehicleTypeId = 1 },
                new VehicleSubType { Id = 6, Name = "Station Wagon", VehicleTypeId = 1 },

                // Motosiklet
                new VehicleSubType { Id = 7, Name = "Naked", VehicleTypeId = 2 },
                new VehicleSubType { Id = 8, Name = "Sport / Racing", VehicleTypeId = 2 },
                new VehicleSubType { Id = 9, Name = "Cruiser", VehicleTypeId = 2 },
                new VehicleSubType { Id = 10, Name = "Scooter", VehicleTypeId = 2 },
                new VehicleSubType { Id = 11, Name = "Touring", VehicleTypeId = 2 },

                // ATV / Arazi Aracı
                new VehicleSubType { Id = 12, Name = "Sport ATV", VehicleTypeId = 3 },
                new VehicleSubType { Id = 13, Name = "Utility ATV", VehicleTypeId = 3 },
                new VehicleSubType { Id = 14, Name = "Quad Bike", VehicleTypeId = 3 },

                // Minivan
                new VehicleSubType { Id = 15, Name = "Minivan", VehicleTypeId = 4 },
                new VehicleSubType { Id = 16, Name = "Panelvan", VehicleTypeId = 4 },

                // Elektrikli Araç
                new VehicleSubType { Id = 17, Name = "Elektrikli Sedan", VehicleTypeId = 5 },
                new VehicleSubType { Id = 18, Name = "Elektrikli SUV", VehicleTypeId = 5 },
                new VehicleSubType { Id = 19, Name = "Elektrikli Hatchback", VehicleTypeId = 5 }
            );

            modelBuilder.Entity<Reservation>()
        .HasOne(r => r.Car)         // Reservation.Car navigation property
        .WithMany()                 // Car için navigation property yoksa WithMany()
        .HasForeignKey(r => r.CarId)
        .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
