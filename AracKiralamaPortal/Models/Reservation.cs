using System;
using System.ComponentModel.DataAnnotations;

namespace AracKiralamaPortal.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        public string UserId { get; set; }  

        [Required]
        public int CarId { get; set; }  

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string Status { get; set; } = "Beklemede"; // Beklemede, Onaylandı, Reddedildi

        public Car? Car { get; set; }
        public ApplicationUser User { get; set; }
    }
}
