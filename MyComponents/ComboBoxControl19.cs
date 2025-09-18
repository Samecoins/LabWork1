using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace MyComponents;

[DefaultEvent(nameof(SelectedValueChanged))]
public partial class ComboBoxControl19 : UserControl
{
    private readonly ComboBox comboBox = new()
    {
        Dock = DockStyle.Fill,
        DropDownStyle = ComboBoxStyle.DropDownList
    };

    [Category("Поведение")]
    [Description("Событие при смене выбранного значения.")]
    [Browsable(true)]
    public event EventHandler? SelectedValueChanged;

    public ComboBoxControl19()
    {
        Controls.Add(comboBox);
        comboBox.SelectedIndexChanged += (s, e) => SelectedValueChanged?.Invoke(this, EventArgs.Empty);
    }

    [Category("Данные")]
    [Description("Заполняет список уникальными непустыми строками.")]
    public void FillItems(IEnumerable<string> items)
    {
        comboBox.BeginUpdate();
        try
        {
            comboBox.Items.Clear();
            foreach (var item in items.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct())
                comboBox.Items.Add(item);
        }
        finally { comboBox.EndUpdate(); }
    }

    [Category("Данные")]
    public void ClearItems() => comboBox.Items.Clear();

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(true)]
    [Category("Данные")]
    [Description("Выбранное значение (если его нет в Items — установка игнорируется).")]
    public string SelectedValue
    {
        get => comboBox.SelectedItem?.ToString() ?? string.Empty;
        set
        {
            if (value is null) return;
            if (comboBox.Items.Contains(value)) comboBox.SelectedItem = value;
        }
    }
}
