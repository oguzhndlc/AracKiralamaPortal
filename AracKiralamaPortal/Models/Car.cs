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

        [Range(1, 10000000, ErrorMessage = "Fiyat 1 ile 10.000.000 arasında olmalıdır.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Lütfen bir marka seçin.")]
        public int BrandId { get; set; }

        [ForeignKey("BrandId")]
        public Brand? Brand { get; set; }

        public string? ImagePath { get; set; }

        [Required(ErrorMessage = "Lütfen Araç Durumunu belirtin.")]
        public bool isAvailable {  get; set; }

    }
}
