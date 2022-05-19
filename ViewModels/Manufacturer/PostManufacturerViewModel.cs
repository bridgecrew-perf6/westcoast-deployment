using System.ComponentModel.DataAnnotations;

namespace Vehicles_API.ViewModels.Manufacturer
{
    public class PostManufacturerViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

    }
}