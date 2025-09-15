﻿namespace ProductManagementApp.ViewModels
{
    public class ProductListViewModel
    {
        public IEnumerable<ProductDisplayViewModel> Products { get; set; }

        // Filter parameters
        public string SearchName { get; set; }
        public string SearchBrand { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // Pagination parameters
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}