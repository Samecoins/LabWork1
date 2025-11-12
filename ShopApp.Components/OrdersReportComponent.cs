using ShopApp.Contract;

namespace ShopApp.Components;
public sealed class OrdersReportComponent : IComponentContract
{
    public string Id => "orders-report";
    public string Title => "Отчёт по заказам";
    public string Category => "Отчёты";
    public UserControl CreateControl() => new OrdersReportControl();
}
