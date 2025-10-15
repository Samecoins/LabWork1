using MyComponents;
using ShopApp.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShopApp.Components;

public partial class OrdersControl : UserControl
{
    // UI
    private readonly SplitContainer split = new() { Dock = DockStyle.Fill, Orientation = Orientation.Vertical, SplitterDistance = 360 };
    private readonly ListBoxControl19 list = new() { Dock = DockStyle.Fill };
    private readonly Button btnAdd = new() { Text = "Добавить", Dock = DockStyle.Top, Height = 32 };
    private readonly Button btnEdit = new() { Text = "Изменить", Dock = DockStyle.Top, Height = 32 };
    private readonly Button btnDel = new() { Text = "Удалить", Dock = DockStyle.Top, Height = 32 };
    private readonly Button btnSave = new() { Text = "Сохранить", Dock = DockStyle.Top, Height = 32 };

    // Data
    private readonly BindingList<Order> orders;
    private readonly BindingList<OrderStatus> statuses;


    private readonly ContextMenuStrip ctx = new();

    public OrdersControl()
    {
        InitializeComponent(); // из .Designer.cs (пусть пустой)

        orders = Storage.LoadOrders();
        statuses = Storage.LoadStatuses();

        list.SetTemplate("Статус {Status} — сумма {Amount} — {Customer} [#{ShortId}]", '{', '}');
        RebindList();

        // Контекстное меню (требование ЛР2)
        var miAdd = new ToolStripMenuItem("Добавить") { ShortcutKeys = Keys.Control | Keys.A };
        var miEdit = new ToolStripMenuItem("Изменить") { ShortcutKeys = Keys.Control | Keys.U };
        var miDel = new ToolStripMenuItem("Удалить") { ShortcutKeys = Keys.Control | Keys.D };
        var miSave = new ToolStripMenuItem("Сохранить") { ShortcutKeys = Keys.Control | Keys.S };

        miAdd.Click += (_, __) => AddOrder();
        miEdit.Click += (_, __) => EditSelected();
        miDel.Click += (_, __) => DeleteSelected();
        miSave.Click += (_, __) => Save();

        ctx.Items.AddRange(new ToolStripItem[] { miAdd, miEdit, miDel, new ToolStripSeparator(), miSave });
        list.ContextMenuStrip = ctx;       // работает, т.к. это Control

        split.Panel1.Controls.Add(list);
        var right = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };
        right.Controls.AddRange(new Control[] { btnSave, btnDel, btnEdit, btnAdd });
        split.Panel2.Controls.Add(right);
        Controls.Add(split);

        btnAdd.Click += (_, __) => AddOrder();
        btnEdit.Click += (_, __) => EditSelected();
        btnDel.Click += (_, __) => DeleteSelected();
        btnSave.Click += (_, __) => Save();

        list.DoubleClick += (_, __) => EditSelected();
    }

    // Глобальная обработка шорткатов (ловит вне зависимости от фокуса)
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        switch (keyData)
        {
            case Keys.Control | Keys.A: AddOrder(); return true;
            case Keys.Control | Keys.U: EditSelected(); return true;
            case Keys.Control | Keys.D: DeleteSelected(); return true;
            case Keys.Control | Keys.S: Save(); return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void RebindList() => list.FillItems(orders);

    private void AddOrder()
    {
        var dlg = new OrderEditForm(statuses);
        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            orders.Add(dlg.Value);
            RebindList();
        }
    }

    private void EditSelected()
    {
        var sel = list.GetSelectedObject<Order>();
        if (sel == null) return;

        var original = orders.FirstOrDefault(o => o.Id == sel.Id);
        if (original == null) return;

        var dlg = new OrderEditForm(statuses, original);
        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            original.Customer = dlg.Value.Customer;
            original.Description = dlg.Value.Description;
            original.Status = dlg.Value.Status;
            original.Amount = dlg.Value.Amount;
            RebindList();
        }
    }


    private void DeleteSelected()
    {
        var sel = list.GetSelectedObject<Order>();
        if (sel == null) return;
        if (MessageBox.Show("Удалить выбранный заказ?", "Подтверждение", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

        var idx = orders.ToList().FindIndex(o => o.Id == sel.Id);
        if (idx >= 0) orders.RemoveAt(idx);
        RebindList();
    }


    private void Save()
    {
        Storage.SaveOrders(orders);
        MessageBox.Show("Сохранено.");
    }
}
