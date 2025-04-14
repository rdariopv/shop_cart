// Services/CartService.cs
using System.Collections.Generic;
using System;
using shop_cart.Models;
namespace shop_cart.Services
{
    

    public class CartService
    {
        public event Action OnChange;

        public int CarritoCantidad { get; private set; } = 0;

        public void AgregarProducto1()
        {
            CarritoCantidad++;
            NotifyStateChanged();
        }
        public void AgregarProducto(Product producto, int cantidad)
        {
            CarritoCantidad += cantidad;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }

}
