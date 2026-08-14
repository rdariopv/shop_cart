using System.Net.Http.Json;
using shop_cart.Components.Layout;
using shop_cart.Models;
using OrderDetail = shop_cart.Models.OrderDetail;

namespace shop_cart.Services
{
    public class OrderService
    {
        private readonly HttpClient _http;

        public OrderService(HttpClient http)
        {
            _http = http;
        }

        /// <summary>
        /// Envía la orden al servidor y devuelve la confirmación.
        /// </summary>
        public async Task<OrderResponse> SubmitOrderAsync(OrderRequest request)
        {
            var response = await _http.PostAsJsonAsync("api/orders", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<OrderResponse>();
                return error ?? new OrderResponse
                {
                    Success = false,
                    Message = $"Error del servidor: {response.StatusCode}"
                };
            }

            return await response.Content.ReadFromJsonAsync<OrderResponse>()
                ?? new OrderResponse { Success = false, Message = "Respuesta vacía." };
        }

        /// <summary>
        /// Obtiene el detalle completo de una orden por su ID.
        /// TODO: Cuando store-api implemente GET /api/orders/{orderId} con BD,
        /// este método simplemente llamará ese endpoint y devolverá los datos reales.
        /// Por ahora devuelve datos estáticos de demostración.
        /// </summary>
        public async Task<Models.OrderDetail?> GetOrderAsync(string orderId)
        {
            // ── TODO: reemplazar con llamada real ────────────────────────────
             return await _http.GetFromJsonAsync<OrderDetail>($"api/orders/{orderId}");
            // ────────────────────────────────────────────────────────────────

            // Datos estáticos de demostración
            await Task.Delay(300); // simula latencia de red

            return new OrderDetail
            {
                OrderId = orderId,
                CreatedAt = DateTime.Now,
                PaymentMethod = "Tarjeta de Crédito",
                Status = "Confirmado",
                Customer = new CustomerInfo
                {
                    NitCi = "Juan",
                    NombreFac = "Pérez",
                    Email = "juan@ejemplo.com"
                },
                Items = new List<OrderItemDto>
                {
                    new() { ProductId = "1", Name = "Producto Demo A", Price = 29.99m, Quantity = 2, Subtotal = 59.98m },
                    new() { ProductId = "2", Name = "Producto Demo B", Price = 15.50m, Quantity = 1, Subtotal = 15.50m },
                    new() { ProductId = "3", Name = "Producto Demo C", Price = 49.00m, Quantity = 1, Subtotal = 49.00m },
                },
                Total = 124.48m
            };
        }
    }
}