using Microsoft.AspNetCore.Http;
using ProductManagementApp.Models;
using System.ComponentModel.DataAnnotations;

namespace ProductManagementApp.ViewModels
{
    public class UpdateProductViewModel
    {
        public int ProductId { get; set; }

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

        // Do NOT mark this as required; it should be optional.
        public IFormFile? MainImageFile { get; set; }

        // For uploading additional related images (optional)
        public List<IFormFile>? RelatedImageFiles { get; set; } = new List<IFormFile>();

        // For displaying the currently saved main image.
        public string? ExistingMainImageFileName { get; set; }

        // For displaying existing related images.
        public List<ProductImage>? ExistingRelatedImages { get; set; } = new List<ProductImage>();
    }
}