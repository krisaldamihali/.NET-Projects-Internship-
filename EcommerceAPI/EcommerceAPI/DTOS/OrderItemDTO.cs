using System.ComponentModel.DataAnnotations;
namespace EcommerceAPI.DTOs
{
    // Data Transfer Object for order items.
    public class OrderItemDTO
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000")]
        public int Quantity { get; set; }
    }
}