namespace shop_cart.Models
{
    public class QrPaymentResult
    {

        public bool Success { get; set; }

        public string? Message { get; set; }

        public string? OrderId { get; set; }

        public string? TransactionId { get; set; }

        public decimal Amount { get; set; }

        public string? QrCode { get; set; }

        public DateTime? ExpiresAt { get; set; }
    }
}
