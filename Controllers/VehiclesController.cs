using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vehicles_API.Data;
using Vehicles_API.Interfaces;
using Vehicles_API.Models;
using Vehicles_API.ViewModels;
using Vehicles_API.ViewModels.Vehicle;

namespace Vehicles_API.Controllers
{
    [ApiController]
    [Route("api/v1/vehicles")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IMapper _mapper;

        public VehiclesController(IVehicleRepository vehicleRepository, IMapper mapper)
        {
            _mapper = mapper;
            _vehicleRepo = vehicleRepository;
        }

        [HttpGet("list")]
        public async Task<ActionResult<List<VehicleViewModel>>> ListVehicles() => Ok(await _vehicleRepo.ListAllVehiclesAsync());

        // Replaced with the method ListManufacturerWithVehicles(int id)
        // [HttpGet("bymake/{manufacturer}")]
        // public async Task<ActionResult<List<VehicleViewModel>>> ListVehiclesByManufacturer(string manufacturer) => Ok();
        // // Ok(await _vehicleRepo.ListVehiclesByManufacturerAsync(manufacturer));

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<VehicleViewModel>> GetVehicleById(int id)
        {
            var response = await _vehicleRepo.GetVehicleAsync(id);

            return response is not null ? Ok(response) : NotFound($"No car was found with Id: {id}");
        }

        [HttpGet]
        [Route("byregno/{regNo}")]
        public async Task<ActionResult<VehicleViewModel>> GetVehicleByRegNo(string regNo)
        {
            var response = await _vehicleRepo.GetVehicleAsync(regNo);

            return response is not null ? Ok(response) : NotFound($"No car was found with RegNo: {regNo}");
        }

        [HttpPost]
        public async Task<ActionResult> AddVehicle(PostVehicleViewModel vehicle)
        {
            try
            {
                if (await _vehicleRepo.GetVehicleAsync(vehicle.RegNo!.ToLower()) is not null)
                    return BadRequest($"A car with registration number {vehicle.RegNo} is already in the system");

                await _vehicleRepo.AddVehicleAsync(vehicle);

                return await _vehicleRepo.SaveAllAsync() ? StatusCode(201) : StatusCode(500, "An error occurred when attempting to save the vehicle");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateVehicle(int id, PostVehicleViewModel vehicle)
        {
            try
            {
                await _vehicleRepo.UpdateVehicleAsync(id, vehicle);

                return await _vehicleRepo.SaveAllAsync() ? NoContent() : StatusCode(500, "An error occurred while updating vehicle");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateVehicle(int id, PatchVehicleViewModel vehicle)
        {
            try
            {
                await _vehicleRepo.UpdateVehicleAsync(id, vehicle);

                return await _vehicleRepo.SaveAllAsync() ? NoContent() : StatusCode(500, "An error occurred while updating the vehicle");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteVehicle(int id)
        {
            await _vehicleRepo.DeleteVehicleAsync(id);

            return await _vehicleRepo.SaveAllAsync() ? NoContent() : StatusCode(500, "An error occurred while deleting the vehicle");
        }
    }
}