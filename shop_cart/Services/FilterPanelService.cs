namespace shop_cart.Services
{
    public class FilterPanelService
    {
        public event Action? OnToggle;

        public void Toggle() => OnToggle?.Invoke();
    }
}
