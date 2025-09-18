using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MyComponents;

public class TemplateFormatException : Exception
{
    public TemplateFormatException(string message) : base(message) { }
}

public partial class ListBoxControl19 : UserControl
{
    private readonly ListBox listBox = new() { Dock = DockStyle.Fill };
    private string template = string.Empty;
    private string startSymbol = "{";
    private string endSymbol = "}";

    private readonly List<object> dataObjects = new();

    public ListBoxControl19()
    {
        Controls.Add(listBox);
    }

    [Category("Данные")]
    [Description("Устанавливает шаблон строки и символы подстановки.")]
    public void SetTemplate(string template, string startSymbol = "{", string endSymbol = "}")
    {
        if (string.IsNullOrWhiteSpace(template) || !template.Contains(startSymbol) || !template.Contains(endSymbol))
            throw new TemplateFormatException("Некорректный шаблон строки.");

        var matches = Regex.Matches(template, $@"\{startSymbol}\w+\{endSymbol}");
        if (matches.Count == 0)
            throw new TemplateFormatException("В шаблоне нет подстановок.");

        this.template = template;
        this.startSymbol = startSymbol;
        this.endSymbol = endSymbol;
    }

    [Category("Данные")]
    [Description("Заполняет список объектами по шаблону.")]
    public void FillItems<T>(IEnumerable<T> objects)
    {
        listBox.BeginUpdate();
        try
        {
            listBox.Items.Clear();
            dataObjects.Clear();
            foreach (var obj in objects)
            {
                listBox.Items.Add(BuildStringFromTemplate(obj!));
                dataObjects.Add(obj!);
            }
        }
        finally { listBox.EndUpdate(); }
    }

    [Category("Данные")]
    public void ClearItems()
    {
        listBox.Items.Clear();
        dataObjects.Clear();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public T? GetSelectedObject<T>()
    {
        if (listBox.SelectedIndex is < 0) return default;
        return (T)dataObjects[listBox.SelectedIndex];
    }

    private string BuildStringFromTemplate(object obj)
    {
        string result = template;
        var type = obj.GetType();
        var matches = Regex.Matches(template, $@"\{startSymbol}(\w+)\{endSymbol}");
        foreach (Match m in matches)
        {
            var name = m.Groups[1].Value;
            var prop = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
            var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public);
            var val = prop?.GetValue(obj) ?? field?.GetValue(obj) ?? "";
            result = result.Replace($"{startSymbol}{name}{endSymbol}", val?.ToString() ?? "");
        }
        return result;
    }
}
