using System.ComponentModel.DataAnnotations;

namespace Vehicles_API.ViewModels.Vehicle
{
    public class PatchVehicleViewModel
    {
        [Required]
        public int ModelYear { get; set; }
        [Required]
        public int Mileage { get; set; }
    }
}