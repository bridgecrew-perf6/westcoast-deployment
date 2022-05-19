using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Vehicles_API.Data;
using Vehicles_API.Interfaces;
using Vehicles_API.Models;
using Vehicles_API.ViewModels.Vehicle;

namespace Vehicles_API.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly VehicleContext _context;
        private readonly IMapper _mapper;

        public VehicleRepository(VehicleContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<VehicleViewModel>> ListAllVehiclesAsync() =>
            await _context.Vehicles.ProjectTo<VehicleViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

        public async Task<VehicleViewModel?> GetVehicleAsync(int id) =>
            await _context.Vehicles.Where(v => v.Id == id)
                .ProjectTo<VehicleViewModel>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

        public async Task<VehicleViewModel?> GetVehicleAsync(string regNumber) =>
            await _context.Vehicles.Where(v => v.RegNo!.ToLower() == regNumber.ToLower())
                .ProjectTo<VehicleViewModel>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

        public async Task AddVehicleAsync(PostVehicleViewModel vehicle)
        {
            var manufacturer = await _context.Manufacturers.Include(m => m.Vehicles)
                .Where(m => m.Name!.ToLower() == vehicle.Manufacturer!.ToLower())
                .SingleOrDefaultAsync();

            if (manufacturer is null)
                throw new Exception($"The specified manufacturer \"{vehicle.Manufacturer}\" does not exist in the database");

            var vehicleToAdd = _mapper.Map<Vehicle>(vehicle);

            vehicleToAdd.Manufacturer = manufacturer;

            await _context.Vehicles.AddAsync(vehicleToAdd);
        }

        public async Task DeleteVehicleAsync(int id)
        {
            var vehicleToDelete = await _context.Vehicles.FindAsync(id);

            if (vehicleToDelete is not null)
                _context.Vehicles.Remove(vehicleToDelete);
        }

        public async Task UpdateVehicleAsync(int id, PostVehicleViewModel vehicle)
        {
            var vehicleToUpdate = await _context.Vehicles.FindAsync(id);

            if (vehicleToUpdate is null)
                throw new Exception($"Could not find a vehicle with id: {id}");

            // _mapper.Map(vehicle, vehicleToUpdate);
            vehicleToUpdate.RegNo = vehicle.RegNo;
            // vehicleToUpdate.Manufacturer = vehicle.Manufacturer;
            vehicleToUpdate.Model = vehicle.Model;
            vehicleToUpdate.ModelYear = vehicle.ModelYear;
            vehicleToUpdate.Mileage = vehicle.Mileage;
            vehicleToUpdate.Value = vehicle.Value;
            vehicleToUpdate.Description = vehicle.Description;
            vehicleToUpdate.ImageUrl = vehicle.ImageUrl;

            _context.Update(vehicleToUpdate);
        }

        public async Task UpdateVehicleAsync(int id, PatchVehicleViewModel vehicle)
        {
            var vehicleToEdit = await _context.Vehicles.FindAsync(id);

            if (vehicleToEdit is null)
                throw new Exception($"Could not find a vehicle with id: {id}");

            if (vehicleToEdit is not null)
            {
                _mapper.Map(vehicle, vehicleToEdit);
                _context.Vehicles.Update(vehicleToEdit);
            }
        }

        public async Task<bool> SaveAllAsync() => await _context.SaveChangesAsync() > 0;
    }
}