using MyComponents;
using ShopApp.Domain;
using System.ComponentModel;
using System.Data;
using System.Globalization;

namespace ShopApp.Components;

public partial class OrderEditForm : Form
{
    // UI
    private readonly TextBox tbCustomer = new() { Dock = DockStyle.Top, PlaceholderText = "ФИО заказчика" };
    private readonly TextBox tbDescription = new() { Dock = DockStyle.Top, Multiline = true, Height = 90, PlaceholderText = "Описание товаров" };
    private readonly ComboBoxControl19 cbStatus = new() { Dock = DockStyle.Top, Height = 28 };
    private readonly TextBox tbAmount = new() { Dock = DockStyle.Top, PlaceholderText = "Сумма (можно пусто)" };

    private readonly Button btnOk = new() { Text = "ОК", DialogResult = DialogResult.OK };
    private readonly Button btnCancel = new() { Text = "Отмена", DialogResult = DialogResult.Cancel };

    // Данные формы
    public Order Value { get; private set; }

    public OrderEditForm(BindingList<OrderStatus> statuses, Order? source = null)
    {
        InitializeComponent(); // важно: т.к. форма создана через дизайнер

        Text = source is null ? "Новый заказ" : "Изменить заказ";
        StartPosition = FormStartPosition.CenterParent;
        Width = 560; Height = 380;

        // Заполнить статусы из справочника
        cbStatus.FillItems(statuses.Select(s => s.Name));

        // Компоновка
        var grid = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 2, RowCount = 6 };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
        grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        grid.Controls.Add(new Label { Text = "ФИО:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        grid.Controls.Add(tbCustomer, 1, 0);
        grid.Controls.Add(new Label { Text = "Описание:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
        grid.Controls.Add(tbDescription, 1, 1);
        grid.Controls.Add(new Label { Text = "Статус:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 2);
        grid.Controls.Add(cbStatus, 1, 2);
        grid.Controls.Add(new Label { Text = "Сумма:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 3);
        grid.Controls.Add(tbAmount, 1, 3);

        var buttons = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill };
        buttons.Controls.AddRange(new Control[] { btnOk, btnCancel });
        grid.Controls.Add(buttons, 0, 5);
        grid.SetColumnSpan(buttons, 2);

        Controls.Add(grid);

        // Инициализация Value из source (или новый)
        Value = source is null ? new Order() : new Order
        {
            Id = source.Id,
            Customer = source.Customer,
            Description = source.Description,
            Status = source.Status,
            Amount = source.Amount
        };

        // Привязка к полям ввода
        tbCustomer.Text = Value.Customer;
        tbDescription.Text = Value.Description;
        cbStatus.SelectedValue = Value.Status;
        tbAmount.Text = Value.Amount?.ToString(CultureInfo.CurrentCulture) ?? string.Empty;

        // Кнопки по умолчанию
        AcceptButton = btnOk;
        CancelButton = btnCancel;

        // Валидация перед закрытием
        btnOk.Click += (_, __) =>
        {
            if (!ValidateAndBind())
                DialogResult = DialogResult.None; // не закрывать, если есть ошибки
        };
    }

    private bool ValidateAndBind()
    {
        var customer = (tbCustomer.Text ?? "").Trim();
        if (string.IsNullOrWhiteSpace(customer))
        {
            MessageBox.Show("Укажите ФИО заказчика.");
            tbCustomer.Focus();
            return false;
        }

        var status = cbStatus.SelectedValue;
        if (string.IsNullOrWhiteSpace(status))
        {
            MessageBox.Show("Выберите статус.");
            return false;
        }

        decimal? amount = null;
        var amountText = (tbAmount.Text ?? "").Trim();
        if (amountText.Length > 0)
        {
            if (!decimal.TryParse(amountText, NumberStyles.Number, CultureInfo.CurrentCulture, out var parsed) || parsed < 0)
            {
                MessageBox.Show("Сумма должна быть числом ≥ 0 или пустой.");
                tbAmount.Focus();
                return false;
            }
            amount = parsed;
        }

        // Сохранить в Value
        Value.Customer = customer;
        Value.Description = (tbDescription.Text ?? "").Trim();
        Value.Status = status;
        Value.Amount = amount;
        return true;
    }
}
