using System.ComponentModel.DataAnnotations;
namespace EcommerceAPI.DTOs
{
    // Data Transfer Object for creating an order.
    public class OrderDTO
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public List<OrderItemDTO> Items { get; set; }
    }
}