﻿using System.ComponentModel.DataAnnotations;
using ECommerceApp.Models;

namespace ECommerceApp.DTOs.RefundDTOs
{
    // DTO for updating refund status Manually
    public class RefundStatusUpdateDTO
    {
        [Required(ErrorMessage = "Refund ID is required.")]
        public int RefundId { get; set; }

        [StringLength(100, ErrorMessage = "Transaction ID cannot exceed 100 characters.")]
        [Required(ErrorMessage = "TransactionId is required.")]
        public string TransactionId { get; set; }

        [Required(ErrorMessage = "Refund Method is required.")]
        public RefundMethod RefundMethod { get; set; }

        [StringLength(500, ErrorMessage = "Refund Reason cannot exceed 500 characters.")]
        public string? RefundReason { get; set; }
        public int? ProcessedBy { get; set; }
    }
}