namespace ProductManagementApp.ViewModels
{
	public class DeleteProductViewModel
	{
		public int ProductId { get; set; }

		// Display the product name for confirmation
		public string Name { get; set; }

		// Optionally show additional details
		public string Brand { get; set; }
		public decimal Price { get; set; }
		public string? MainImageFileName { get; set; }

		// You can include a short description or key features if desired
		public string Description { get; set; }
	}
}