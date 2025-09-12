﻿namespace ECommerceApp.DTOs.RefundDTOs
{
    // DTO for pending refund details.
    public class PendingRefundResponseDTO
    {
        public int CancellationId { get; set; }
        public int OrderId { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal CancellationCharge { get; set; }
        public decimal ComputedRefundAmount { get; set; }
        public string? CancellationRemarks { get; set; }
    }
}