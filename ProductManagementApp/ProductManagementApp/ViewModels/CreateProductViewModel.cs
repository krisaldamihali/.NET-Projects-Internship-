using System.ComponentModel.DataAnnotations;
namespace ProductManagementApp.ViewModels
{
    public class CreateProductViewModel
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(100)]
        public string Brand { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, 100)]
        public decimal Discount { get; set; }

        // File upload fields
        public IFormFile MainImageFile { get; set; }
        public List<IFormFile> RelatedImageFiles { get; set; } = new();
    }
}