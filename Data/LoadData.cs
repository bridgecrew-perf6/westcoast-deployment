using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Vehicles_API.Models;
using Vehicles_API.ViewModels.Vehicle;

namespace Vehicles_API.Data
{
    public class LoadData
    {
        public static async Task LoadManufacturers(VehicleContext context)
        {
            if (await context.Manufacturers.AnyAsync()) return;

            var makeData = await File.ReadAllTextAsync("Data/make.json");
            var manufacturers = JsonSerializer.Deserialize<List<Manufacturer>>(makeData);

            await context.Manufacturers.AddRangeAsync(manufacturers!);
            await context.SaveChangesAsync();

        }
        public static async Task LoadVehicles(VehicleContext context)
        {
            if (await context.Vehicles.AnyAsync()) return;

            var vehicleData = await File.ReadAllTextAsync("Data/vehicles.json");
            var vehicles = JsonSerializer.Deserialize<List<PostVehicleViewModel>>(vehicleData);

            if (vehicles is null) return;

            foreach (var vehicle in vehicles)
            {
                var manufacturer = await context.Manufacturers.Where(m => m.Name.ToLower() == vehicle.Manufacturer!.ToLower()).SingleOrDefaultAsync();
                if (manufacturer is not null)
                {
                    var newVehicle = new Vehicle
                    {
                        RegNo = vehicle.RegNo,
                        Model = vehicle.Model,
                        ModelYear = vehicle.ModelYear,
                        Mileage = vehicle.Mileage,
                        ImageUrl = vehicle.ImageUrl,
                        Description = vehicle.Description,
                        Value = vehicle.Value,
                        Manufacturer = manufacturer,
                    };

                    context.Vehicles.Add(newVehicle);
                }
            }
            await context.SaveChangesAsync();
        }
    }
}