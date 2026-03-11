using shop_cart.Models;

namespace shop_cart.Services
{
    public class CartState
    {
        private List<CartItem> items = new();

        public event Action? OnChange;

        public IReadOnlyList<CartItem> Items => items.AsReadOnly();

        public int Count => items.Sum(x => x.Quantity);

        public void SetItems(List<CartItem> newItems)
        {
            items = newItems;
            NotifyStateChanged();
        }

        public void Add(Product product, int quantity)
        {
            var existing = items.FirstOrDefault(x => x.ProductItem.Id == product.Id);

            if (existing != null)
                existing.Quantity += quantity;
            else
                items.Add(new CartItem
                {
                    ProductItem = product,
                    Name = product.Nombre,
                    Price = product.Precio,
                    Quantity = quantity
                });

            NotifyStateChanged();
        }

        public void Increment(CartItem item)
        {
            var existing = items.FirstOrDefault(x => x.ProductItem.Id == item.ProductItem.Id);
            if (existing == null) return;

            existing.Quantity++;
            NotifyStateChanged();
        }
        public void Decrement(CartItem item)
        {
            var existing = items.FirstOrDefault(x => x.ProductItem.Id == item.ProductItem.Id);
            if (existing == null) return;

            if (existing.Quantity > 1)
                existing.Quantity--;
            else
                items.Remove(existing);

            NotifyStateChanged();
        }

        public void Remove(CartItem item)
        {
            items.Remove(item);
            NotifyStateChanged();
        }

        public void Clear()
        {
            items.Clear();
            NotifyStateChanged();
        }

        private void NotifyStateChanged()
        {
            OnChange?.Invoke();
        }
    }
}
