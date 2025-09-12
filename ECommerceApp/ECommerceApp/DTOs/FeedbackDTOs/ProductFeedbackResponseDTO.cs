﻿namespace ECommerceApp.DTOs.FeedbackDTOs
{
    // DTO for returning feedback details along with average rating for a product
    public class ProductFeedbackResponseDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public double AverageRating { get; set; }
        public List<CustomerFeedback> Feedbacks { get; set; }
    }

    public class CustomerFeedback
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } // Combines FirstName and LastName
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}