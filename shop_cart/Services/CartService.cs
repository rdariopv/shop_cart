// Services/CartService.cs
using System.Collections.Generic;
using System;
using shop_cart.Models;
using QRCoder;
namespace shop_cart.Services
{
    

    public class CartService
    {
        private List<CartItem> items = new List<CartItem>();
        public event Action OnChange;

        public int CarritoCantidad { get; private set; } = 0;

       
        public void AgregarProducto(Product producto, int cantidad)
        {
            var existingItem = items.FirstOrDefault(i => i.ProductItem.Id  == producto.Id  );
            //CarritoCantidad += cantidad;

            if (existingItem != null)
            {
                existingItem.Quantity += cantidad;
            }
            else
            {
                items.Add(new CartItem
                {
                    ProductItem = producto,
                    Name = producto.Nombre,
                    Price = producto.Precio,
                    Quantity = cantidad
                });
            }
            CarritoCantidad= items.Sum(i => i.Quantity);
            Console.WriteLine($"Carrito ahora tiene {CarritoCantidad} items");
            NotifyStateChanged();
        }
        public void RemoveItem(CartItem item)
        {
            items.Remove(item);
            CarritoCantidad = items.Sum(i => i.Quantity);
            NotifyStateChanged();
        }
        public List<CartItem> GetCartItems()
        {
            return items;
        }

        public int GetCartCount()
        {
            return items.Sum(i => i.Quantity);
        }
        private void NotifyStateChanged() => OnChange?.Invoke();

        public string GenerateQrAsBase64(string content)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrBytes = qrCode.GetGraphic(20);
            return "data:image/png;base64," + Convert.ToBase64String(qrBytes);
        }
    }

}
