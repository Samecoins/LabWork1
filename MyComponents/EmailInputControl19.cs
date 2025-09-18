using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MyComponents;

public class EmailValidationException : Exception
{
    public EmailValidationException(string message) : base(message) { }
}

[DefaultEvent(nameof(ValueChanged))]
public partial class EmailInputControl19 : UserControl
{
    private readonly TextBox textBox = new() { Dock = DockStyle.Fill };
    private readonly ToolTip toolTip = new();

    [Category("Поведение")]
    [Description("Событие при изменении текста.")]
    public event EventHandler? ValueChanged;

    public EmailInputControl19()
    {
        Controls.Add(textBox);
        textBox.TextChanged += (s, e) => ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    [Category("Валидация")]
    [Description("Регулярное выражение для проверки e-mail.")]

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Pattern { get; set; } = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    [Category("Валидация")]
    [Description("Устанавливает пример корректного ввода (ToolTip).")]
    public void SetExample(string example) => toolTip.SetToolTip(textBox, $"Пример: {example}");

    [Category("Данные")]
    [Description("Значение e-mail; при несоответствии шаблону генерируется EmailValidationException.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Value
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Pattern))
                throw new EmailValidationException("Не задан шаблон проверки e-mail.");
            if (!Regex.IsMatch(textBox.Text, Pattern))
                throw new EmailValidationException("Введённый e-mail не соответствует шаблону.");
            return textBox.Text;
        }
        set
        {
            if (string.IsNullOrWhiteSpace(Pattern))
                throw new EmailValidationException("Не задан шаблон проверки e-mail.");
            if (!Regex.IsMatch(value ?? "", Pattern))
                throw new EmailValidationException("Присвоенный e-mail не соответствует шаблону.");
            textBox.Text = value!;
        }
    }
}
