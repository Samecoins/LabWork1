using OrdersStatusMicroservice.Models;

namespace OrdersStatusMicroservice.Data;

public static class FakeDb
{
    public static List<OrderStatus> Statuses { get; } = new()
    {
        new OrderStatus { Id = 1, Name = "Доставлен" },
        new OrderStatus { Id = 2, Name = "Отменён" },
        new OrderStatus { Id = 3, Name = "Ожидает оплаты" },
    };
}
