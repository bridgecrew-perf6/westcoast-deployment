using AutoMapper;
using Vehicles_API.Models;
using Vehicles_API.ViewModels.Manufacturer;
using Vehicles_API.ViewModels.Vehicle;

namespace Vehicles_API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Map from -> to
            CreateMap<PostVehicleViewModel, Vehicle>()
                .ForMember(dest => dest.Manufacturer, opt => opt.Ignore());
            CreateMap<Vehicle, VehicleViewModel>()
                .ForMember(dest => dest.VehicleId, options => options.MapFrom(src => src.Id))
                .ForMember(dest => dest.VehicleName, options => options.MapFrom(src => string.Concat(src.Manufacturer.Name, " ", src.Model)));
            CreateMap<PatchVehicleViewModel, Vehicle>();

            CreateMap<PostManufacturerViewModel, Manufacturer>();
            CreateMap<Manufacturer, ManufacturerViewModel>()
                .ForMember(dest => dest.ManufacturerId, options => options
                    .MapFrom(src => src.Id));

        }
    }
}