using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApp.Domain;
public sealed class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Customer { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // текст статуса
    public decimal? Amount { get; set; } 


    // Удобное краткое представление Id для отображения
    public string ShortId => Id.ToString()[..8];
}


public sealed class OrderStatus
{
    public string Name { get; set; } = string.Empty;
}
