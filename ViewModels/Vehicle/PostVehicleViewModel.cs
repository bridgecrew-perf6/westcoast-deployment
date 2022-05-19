using System.ComponentModel.DataAnnotations;

namespace Vehicles_API.ViewModels.Vehicle
{
    public class PostVehicleViewModel
    {
        [Required(ErrorMessage = "Registration number is required")]
        public string? RegNo { get; set; }
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public int ModelYear { get; set; }
        public int Mileage { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public int Value { get; set; }
    }
}