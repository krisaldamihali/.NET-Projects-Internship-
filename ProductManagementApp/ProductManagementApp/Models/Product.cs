using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManagementApp.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        // Additional fields
        [StringLength(100)]
        public string Brand { get; set; }

        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Range(0, 100)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; } // e.g., store as 0-100 for % discount

        // Image handling
        public string? MainImageFileName { get; set; }
        public byte[]? MainImageData { get; set; }

        // Navigation: Related images
        public ICollection<ProductImage> RelatedImages { get; set; }
    }
}