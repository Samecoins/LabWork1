using MyComponents;
using ShopApp.Domain;
using System.ComponentModel;
using System.Data;

namespace ShopApp.Components;

public partial class OrdersReportControl : UserControl
{
    // UI
    private readonly ComboBoxControl19 cbStatus = new() { Dock = DockStyle.Top, Height = 28 };
    private readonly DataGridView grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };

    // Data
    private readonly BindingList<Order> orders;
    private readonly BindingList<OrderStatus> statuses;

    public OrdersReportControl()
    {
        InitializeComponent();

        orders = Storage.LoadOrders();
        statuses = Storage.LoadStatuses();

        cbStatus.FillItems(new[] { "(все)" }.Concat(statuses.Select(s => s.Name)));
        cbStatus.SelectedValueChanged += (_, __) => ApplyFilter();

        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2 };
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        panel.Controls.Add(cbStatus, 0, 0);
        panel.Controls.Add(grid, 0, 1);
        Controls.Add(panel);

        grid.DataSource = new BindingList<Order>(orders.ToList());
        HideTechnicalColumns();
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var sel = cbStatus.SelectedValue;
        IEnumerable<Order> q = orders;
        if (!string.IsNullOrWhiteSpace(sel) && sel != "(все)")
            q = q.Where(o => o.Status == sel);

        grid.DataSource = new BindingList<Order>(q.ToList());
        HideTechnicalColumns(); // важно: после каждого ребинда
    }

    private void HideTechnicalColumns()
    {
        if (grid.Columns.Contains(nameof(Order.Id)))
            grid.Columns[nameof(Order.Id)].Visible = false;
        // при желании можно спрятать ещё что-то служебное:
        // if (grid.Columns.Contains(nameof(Order.ShortId))) grid.Columns[nameof(Order.ShortId)].Visible = false;
    }
}
