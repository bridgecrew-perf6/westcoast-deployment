using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Vehicles_API.Data;
using Vehicles_API.Interfaces;
using Vehicles_API.Models;
using Vehicles_API.ViewModels.Manufacturer;
using Vehicles_API.ViewModels.Vehicle;

namespace Vehicles_API.Repositories
{
    public class ManufacturerRepository : IManufacturerRepository
    {
        private readonly VehicleContext _context;
        private readonly IMapper _mapper;
        public ManufacturerRepository(VehicleContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<ManufacturerViewModel>> ListAllManufacturersAsync() =>
            await _context.Manufacturers.ProjectTo<ManufacturerViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

        public async Task<IEnumerable<ManufacturerWithVehiclesViewModel>> ListManufacturersVehiclesAsync()
        {
            // Project manufacturer data to view model
            return await _context.Manufacturers.Include(m => m.Vehicles).Select(m => new ManufacturerWithVehiclesViewModel
            {
                ManufacturerId = m.Id,
                Name = m.Name,
                Vehicles = m.Vehicles
                // Project other end of our join (vehicle data) to a VehicleViewModel
                    .Select(v => new VehicleViewModel
                    {
                        VehicleId = v.Id,
                        RegNo = v.RegNo,
                        VehicleName = string.Concat(v.Manufacturer.Name, " ", v.Model),
                        ModelYear = v.ModelYear,
                        Mileage = v.Mileage
                    }).ToList()
            }).ToListAsync();
        }

        public async Task<ManufacturerWithVehiclesViewModel?> ListManufacturersVehiclesAsync(int id)
        {
            // Project manufacturer data to view model
            return await _context.Manufacturers.Where(m => m.Id == id).Include(m => m.Vehicles).Select(m => new ManufacturerWithVehiclesViewModel
            {
                ManufacturerId = m.Id,
                Name = m.Name,
                Vehicles = m.Vehicles
                // Project other end of our join (vehicle data) to a VehicleViewModel
                    .Select(v => new VehicleViewModel
                    {
                        VehicleId = v.Id,
                        RegNo = v.RegNo,
                        VehicleName = string.Concat(v.Manufacturer.Name!, " ", v.Model!),
                        ModelYear = v.ModelYear,
                        Mileage = v.Mileage
                    }).ToList()
            }).SingleOrDefaultAsync();
        }

        public async Task<ManufacturerViewModel?> GetManufacturerAsync(int id) =>
            await _context.Manufacturers.Where(m => m.Id == id)
                .ProjectTo<ManufacturerViewModel>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

        public async Task<ManufacturerViewModel?> GetManufacturerAsync(string name) =>
            await _context.Manufacturers.Where(m => m.Name!.ToLower() == name.ToLower())
                .ProjectTo<ManufacturerViewModel>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

        public async Task AddManufacturerAsync(PostManufacturerViewModel manufacturer)
        {
            var manufacturerToAdd = _mapper.Map<Manufacturer>(manufacturer);
            await _context.Manufacturers.AddAsync(manufacturerToAdd);
        }

        public async Task UpdateManufacturerAsync(int id, PostManufacturerViewModel manufacturer)
        {
            var manufacturerToUpdate = await _context.Manufacturers.FindAsync(id);

            if (manufacturerToUpdate is null)
                throw new Exception($"There is no manufacturer with id: {id} in the database");

            _mapper.Map(manufacturer, manufacturerToUpdate);
            _context.Manufacturers.Update(manufacturerToUpdate);
        }

        public async Task DeleteManufacturerAsync(int id)
        {
            var manufacturerToDelete = await _context.Manufacturers.FindAsync(id);

            if (manufacturerToDelete is null)
                throw new Exception($"There is no manufacturer with id: {id} in the database");

            _context.Manufacturers.Remove(manufacturerToDelete);
        }

        public async Task<bool> SaveAllAsync() => await _context.SaveChangesAsync() > 0;


    }
}