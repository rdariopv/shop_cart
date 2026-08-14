namespace shop_cart.Models
{
    /// <summary>
    /// Modelo completo de una orden para mostrar en el comprobante.
    /// TODO: Este modelo debe ser devuelto por GET /api/orders/{orderId}
    /// cuando se implemente la persistencia en BD.
    /// </summary>
    public class OrderDetail
    {
        public string OrderId { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public string PaymentMethod { get; set; } = "";
        public string Status { get; set; } = "Confirmado";
        public CustomerInfo Customer { get; set; } = new();
        public List<OrderItemDto> Items { get; set; } = new();
        public decimal Total { get; set; }
    }
}
