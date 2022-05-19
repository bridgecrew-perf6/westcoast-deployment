using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vehicles_API.Interfaces;
using Vehicles_API.ViewModels.Manufacturer;

namespace Vehicles_API.Controllers
{
    [ApiController]
    [Route("api/v1/manufacturers")]
    [Authorize]
    public class ManufacturersController : ControllerBase
    {
        private readonly IManufacturerRepository _manufacturerRepo;
        private readonly IMapper _mapper;
        public ManufacturersController(IManufacturerRepository manufacturerRepo, IMapper mapper)
        {
            _mapper = mapper;
            _manufacturerRepo = manufacturerRepo;
        }

        [HttpGet]
        public async Task<ActionResult<List<ManufacturerViewModel>>> ListAllManufacturers() => Ok(await _manufacturerRepo.ListAllManufacturersAsync());

        [HttpGet("vehicles")]
        public async Task<ActionResult> ListManufacturersAndVehicles() => Ok(await _manufacturerRepo.ListManufacturersVehiclesAsync());

        [HttpGet("{id}/vehicles")]
        public async Task<ActionResult> ListManufacturerVehicles(int id)
        {
            var result = await _manufacturerRepo.ListManufacturersVehiclesAsync(id);
            return result is not null ? Ok(result) : NotFound($"There is no manufacturer with id: {id}");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ManufacturerViewModel>> GetManufacturerById(int id)
        {
            var response = await _manufacturerRepo.GetManufacturerAsync(id);

            return response is not null ? Ok(response) : NotFound($"No manufacturer was found with Id: {id}");
        }

        [HttpGet("byname/{name}")]
        public async Task<ActionResult<ManufacturerViewModel>> GetManufacturerByName(string name)
        {
            var response = await _manufacturerRepo.GetManufacturerAsync(name);

            return response is not null ? Ok(response) : NotFound($"No manufacturer named {name} was found");
        }

        [HttpPost]
        public async Task<ActionResult<ManufacturerViewModel>> AddManufacturer(PostManufacturerViewModel manufacturer)
        {
            await _manufacturerRepo.AddManufacturerAsync(manufacturer);

            if (await _manufacturerRepo.GetManufacturerAsync(manufacturer.Name!.ToLower()) is not null)
                return BadRequest($"The manufacturer {manufacturer.Name} is already in the database");

            // Check if manufacturer exists in the database already

            return await _manufacturerRepo.SaveAllAsync() ? StatusCode(201) : StatusCode(500, "An error occured while saving the manufacturer");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateManufacturer(int id, PostManufacturerViewModel manufacturer)
        {
            try
            {
                await _manufacturerRepo.UpdateManufacturerAsync(id, manufacturer);

                if (await _manufacturerRepo.GetManufacturerAsync(manufacturer.Name!.ToLower()) is not null)
                    return BadRequest($"There is already a manufacturer called {manufacturer.Name} in the database");

                return await _manufacturerRepo.SaveAllAsync() ? NoContent() : StatusCode(500, "An error occurred while attempting updating the database");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteManufacturer(int id)
        {
            try
            {
                await _manufacturerRepo.DeleteManufacturerAsync(id);

                return await _manufacturerRepo.SaveAllAsync() ? NoContent() : BadRequest("An error occurred while deleting the manufacturer");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
    }
}