using ShopApp.Domain;
using System.ComponentModel;

namespace ShopApp.Components;

public partial class StatusDirectoryControl : UserControl
{
    // UI
    private readonly DataGridView grid = new() { Dock = DockStyle.Fill, AutoGenerateColumns = false, AllowUserToAddRows = false };
    private readonly ToolStrip tool = new() { GripStyle = ToolStripGripStyle.Hidden };
    private readonly ToolStripButton btnAdd = new("Добавить");
    private readonly ToolStripButton btnDel = new("Удалить");
    private readonly ToolStripButton btnSave = new("Сохранить");

    // Data
    private readonly BindingList<OrderStatus> model;

    public StatusDirectoryControl()
    {
        InitializeComponent(); // из .Designer.cs (пусть он пустой)
        model = Storage.LoadStatuses();

        // Grid
        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(OrderStatus.Name),
            HeaderText = "Статус",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });
        grid.DataSource = model;

        // Toolbar
        tool.Items.AddRange(new ToolStripItem[] { btnAdd, btnDel, new ToolStripSeparator(), btnSave });

        // Layout
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2 };
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        panel.Controls.Add(tool, 0, 0);
        panel.Controls.Add(grid, 0, 1);
        Controls.Add(panel);

        // Events
        btnAdd.Click += (_, __) => { model.Add(new OrderStatus { Name = "" }); grid.CurrentCell = grid.Rows[^1].Cells[0]; grid.BeginEdit(true); };
        btnDel.Click += (_, __) => DeleteSelected();
        btnSave.Click += (_, __) => Save();

        grid.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Insert) { btnAdd.PerformClick(); e.Handled = true; }
            if (e.KeyCode == Keys.Delete) { btnDel.PerformClick(); e.Handled = true; }
        };
    }

    private void DeleteSelected()
    {
        if (grid.CurrentRow == null) return;
        if (MessageBox.Show("Удалить выбранную строку?", "Подтверждение", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
        model.RemoveAt(grid.CurrentRow.Index);
    }


    private void Save()
    {
        for (int i = model.Count - 1; i >= 0; i--)
            if (string.IsNullOrWhiteSpace(model[i].Name)) model.RemoveAt(i);
        Storage.SaveStatuses(model);
        MessageBox.Show("Сохранено.");
    }
}
