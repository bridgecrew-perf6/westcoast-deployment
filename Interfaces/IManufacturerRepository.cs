using Vehicles_API.Models;
using Vehicles_API.ViewModels.Manufacturer;

namespace Vehicles_API.Interfaces
{
    public interface IManufacturerRepository
    {
        Task<IEnumerable<ManufacturerViewModel>> ListAllManufacturersAsync();
        Task<IEnumerable<ManufacturerWithVehiclesViewModel>> ListManufacturersVehiclesAsync();
        Task<ManufacturerWithVehiclesViewModel?> ListManufacturersVehiclesAsync(int id);
        Task<ManufacturerViewModel?> GetManufacturerAsync(int id);
        Task<ManufacturerViewModel?> GetManufacturerAsync(string name);
        Task AddManufacturerAsync(PostManufacturerViewModel manufacturer);
        Task UpdateManufacturerAsync(int id, PostManufacturerViewModel manufacturer);
        Task DeleteManufacturerAsync(int id);
        Task<bool> SaveAllAsync();
    }
}