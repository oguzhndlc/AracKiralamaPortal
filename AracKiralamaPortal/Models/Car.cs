using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AracKiralamaPortal.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Model alanı zorunludur.")]
        public string Model { get; set; }

        [Range(1950, 2100, ErrorMessage = "Yıl 1950 ile 2100 arasında olmalıdır.")]
        public int Year { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(1, 10_000_000, ErrorMessage = "Fiyat 1 ile 10.000.000 arasında olmalıdır.")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }


        [Required(ErrorMessage = "Lütfen bir marka seçin.")]
        public int BrandId { get; set; }

        [ForeignKey("BrandId")]
        public Brand? Brand { get; set; }

        public string? ImagePath { get; set; }

        [Required(ErrorMessage = "Lütfen Araç Durumunu belirtin.")]
        public bool isAvailable { get; set; }

        // Yeni eklemeler
        [Required(ErrorMessage = "Yakıt tipi alanı zorunludur.")]
        public string FuelType { get; set; }

        [Required(ErrorMessage = "Vites tipi alanı zorunludur.")]
        public string GearType { get; set; }

        [Range(0, 1000000, ErrorMessage = "Kilometre 0 ile 1.000.000 arasında olmalıdır.")]
        public int Mileage { get; set; } // km

        [Range(1, 2000, ErrorMessage = "Motor gücü 1 ile 2000 hp arasında olmalıdır.")]
        public int EnginePower { get; set; } // hp

        [Range(50, 10000)]
        [Required(ErrorMessage = "Motor hacmi 50 ile 10.000 cc arasında olmalıdır.")]
        public int EngineCapacity { get; set; } // cc

        [Required(ErrorMessage = "Lütfen bir renk seçin")]
        public string Color { get; set; }


        public bool HasAC { get; set; }
        public bool HasGPS { get; set; }
    }
}
