using shop_cart.Models;

namespace shop_cart.Services
{
    public class PaymentService
    {
        private readonly HttpClient _httpClient;
        public PaymentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<QrPaymentResult?> CreateQrPaymentAsync(
            string orderId,
            decimal amount)
        {
            var request = new
            {
                orderId,
                amount
            };

            var response = await _httpClient.PostAsJsonAsync(
                "payments/qr",
                request);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<QrPaymentResult>();
        }
    }
}

