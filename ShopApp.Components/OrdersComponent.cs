using ShopApp.Contract;

namespace ShopApp.Components;

public sealed class OrdersComponent : IComponentContract
{
    public string Id => "orders";
    public string Title => "Заказы";
    public string Category => "Справочники";
    public UserControl CreateControl() => new OrdersControl();
}
