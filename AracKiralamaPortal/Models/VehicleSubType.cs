namespace AracKiralamaPortal.Models
{
    public class VehicleSubType
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public int VehicleTypeId { get; set; }
        public VehicleType VehicleType { get; set; } = null!;
    }

}
