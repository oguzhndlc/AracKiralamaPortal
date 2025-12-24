namespace AracKiralamaPortal.Models
{
    public class VehicleType
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<VehicleSubType> SubTypes { get; set; } = new List<VehicleSubType>();
    }
}
