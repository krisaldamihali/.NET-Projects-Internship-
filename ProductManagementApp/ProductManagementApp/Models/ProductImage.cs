using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManagementApp.Models
{
    public class ProductImage
    {
        [Key]
        public int ImageId { get; set; } //Primary Key

        [Required]
        public int ProductId { get; set; }

        public string ImageFileName { get; set; }
        public byte[] ImageData { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}