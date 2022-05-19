using Vehicles_API.Models;
using Vehicles_API.ViewModels.Vehicle;

namespace Vehicles_API.Interfaces
{
    public interface IVehicleRepository
    {
        Task<IEnumerable<VehicleViewModel>> ListAllVehiclesAsync();
        Task<VehicleViewModel?> GetVehicleAsync(int id);
        Task<VehicleViewModel?> GetVehicleAsync(string regNumber);
        Task AddVehicleAsync(PostVehicleViewModel vehicle);
        Task DeleteVehicleAsync(int id);
        Task UpdateVehicleAsync(int id, PostVehicleViewModel vehicle);
        Task UpdateVehicleAsync(int id, PatchVehicleViewModel vehicle);
        Task<bool> SaveAllAsync();
    }
}