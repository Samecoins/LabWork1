using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShopApp.Domain;
public static class Storage
{
    static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };


    static string DataDir => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ShopAppDemo");
    static string OrdersPath => Path.Combine(DataDir, "orders.json");
    static string StatusesPath => Path.Combine(DataDir, "statuses.json");


    public static BindingList<OrderStatus> LoadStatuses()
    {
        Directory.CreateDirectory(DataDir);
        if (!File.Exists(StatusesPath))
        {
            // Стартовый набор
            var seed = new BindingList<OrderStatus>
{
new() { Name = "Новый" },
new() { Name = "В обработке" },
new() { Name = "Доставлен" }
};
            SaveStatuses(seed);
            return seed;
        }
        var json = File.ReadAllText(StatusesPath);
        return JsonSerializer.Deserialize<BindingList<OrderStatus>>(json, JsonOpts) ?? new();
    }


    public static void SaveStatuses(BindingList<OrderStatus> list)
    {
        Directory.CreateDirectory(DataDir);
        var json = JsonSerializer.Serialize(list, JsonOpts);
        File.WriteAllText(StatusesPath, json);
    }


    public static BindingList<Order> LoadOrders()
    {
        Directory.CreateDirectory(DataDir);
        if (!File.Exists(OrdersPath))
        {
            var empty = new BindingList<Order>();
            SaveOrders(empty);
            return empty;
        }
        var json = File.ReadAllText(OrdersPath);
        return JsonSerializer.Deserialize<BindingList<Order>>(json, JsonOpts) ?? new();
    }


    public static void SaveOrders(BindingList<Order> list)
    {
        Directory.CreateDirectory(DataDir);
        var json = JsonSerializer.Serialize(list, JsonOpts);
        File.WriteAllText(OrdersPath, json);
    }
}
