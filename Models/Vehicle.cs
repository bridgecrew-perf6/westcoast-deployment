using System.ComponentModel.DataAnnotations.Schema;

namespace Vehicles_API.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string? RegNo { get; set; }
        public int ManufacturerId { get; set; }
        public string? Model { get; set; }
        public int ModelYear { get; set; }
        public int Mileage { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public int Value { get; set; }
        [ForeignKey("ManufacturerId")]
        public Manufacturer Manufacturer { get; set; } = new();
    }
}