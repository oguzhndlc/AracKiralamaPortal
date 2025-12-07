using System.ComponentModel.DataAnnotations;

namespace AracKiralamaPortal.Models
{
    public class Brand
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Marka adı zorunludur.")]
        [MinLength(2, ErrorMessage = "Marka adı en az 2 karakter olmalıdır.")]
        public string Name { get; set; }
    }
}
