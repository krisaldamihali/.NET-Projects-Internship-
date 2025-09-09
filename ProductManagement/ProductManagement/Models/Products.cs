namespace ProductManagement.Models
{
    public class Product
    {
        public int Id { get; set; }                              // Unique product identifier
        public string Name { get; set; } = string.Empty;         // Product name
        public string Description { get; set; } = string.Empty;  // Detailed description
        public decimal Price { get; set; }                       // Product price
        public string Category { get; set; } = string.Empty;     // Product category (e.g., Electronics, Apparel)
        public int StockQuantity { get; set; }                   // Available stock quantity
        public string ImageUrl { get; set; } = string.Empty;     // URL to product image
        public DateTime CreatedDate { get; set; }                // Date when product was added
        public bool IsActive { get; set; } = true;               // Product availability status
    }
}