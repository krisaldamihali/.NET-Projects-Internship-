﻿using Microsoft.EntityFrameworkCore;
using ECommerceApp.Data;
using ECommerceApp.Models;
namespace ECommerceApp.Services
{
    public class RefundProcessingBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        // Run the background task every 5 minutes (adjust as needed).
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);
        public RefundProcessingBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessPendingAndFailedRefundsAsync(stoppingToken);
                await Task.Delay(_interval, stoppingToken);
            }
        }
        private async Task ProcessPendingAndFailedRefundsAsync(CancellationToken cancellationToken)
        {
            // Create a new scope to use scoped services.
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();
                var refundService = scope.ServiceProvider.GetRequiredService<RefundService>();
                // Query refunds with status Pending or Failed.
                var refunds = await context.Refunds
                .Include(r => r.Cancellation)
                .ThenInclude(c => c.Order)
                .ThenInclude(o => o.Customer)
                .Include(r => r.Payment)
                .Where(r => r.Status == RefundStatus.Pending || r.Status == RefundStatus.Failed)
                .ToListAsync(cancellationToken);
                foreach (var refund in refunds)
                {
                    var gatewayResponse = await refundService.ProcessRefundPaymentAsync(refund);
                    if (gatewayResponse.IsSuccess)
                    {
                        refund.Status = RefundStatus.Completed;
                        refund.TransactionId = gatewayResponse.TransactionId;
                        refund.CompletedAt = DateTime.UtcNow;
                        refund.Payment.Status = PaymentStatus.Refunded;
                        context.Payments.Update(refund.Payment);
                        context.Refunds.Update(refund);
                        await context.SaveChangesAsync(cancellationToken);
                        if (refund.Cancellation?.Order?.Customer != null &&
                        !string.IsNullOrEmpty(refund.Cancellation.Order.Customer.Email))
                        {
                            await emailService.SendEmailAsync(
                            refund.Cancellation.Order.Customer.Email,
                            $"Your Refund Has Been Processed Successfully, Order #{refund.Cancellation.Order.OrderNumber}",
                            refundService.GenerateRefundSuccessEmailBody(refund, refund.Cancellation.Order.OrderNumber, refund.Cancellation),
                            IsBodyHtml: true);
                        }
                    }
                    else
                    {
                        refund.Status = gatewayResponse.Status;
                        refund.CompletedAt = DateTime.UtcNow;
                        await context.SaveChangesAsync(cancellationToken);
                    }
                }
            }
        }
    }
}