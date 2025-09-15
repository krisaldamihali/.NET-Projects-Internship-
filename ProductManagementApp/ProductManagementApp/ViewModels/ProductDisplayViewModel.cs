namespace ProductManagementApp.ViewModels
{
    public class ProductDisplayViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public string? MainImageFileName { get; set; }
        public List<ProductRelatedImageDisplayViewModel> RelatedImages { get; set; }
            = new List<ProductRelatedImageDisplayViewModel>();
    }

    public class ProductRelatedImageDisplayViewModel
    {
        public int ImageId { get; set; }
        public string ImageFileName { get; set; }
    }
}