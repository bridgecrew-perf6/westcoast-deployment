using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vehicles_API.ViewModels.Vehicle;

namespace Vehicles_API.ViewModels.Manufacturer
{
    public class ManufacturerWithVehiclesViewModel
    {
        public int ManufacturerId { get; set; }
        public string? Name { get; set; }
        public List<VehicleViewModel> Vehicles { get; set; } = new();
    }
}