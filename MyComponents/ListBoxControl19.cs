using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
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
    private char startSymbol = '{';
    private char endSymbol = '}';

    // Обёртка: хранит отображаемый текст и исходный объект в самом ListBox
    private sealed class ItemWrap
    {
        public ItemWrap(string display, object source) { Display = display; Source = source; }
        public string Display { get; }
        public object Source { get; }
        public override string ToString() => Display;
    }

    public ListBoxControl19()
    {
        Controls.Add(listBox);
    }

    [Category("Данные")]
    [Description("Устанавливает шаблон строки и символы подстановки.")]
    public void SetTemplate(string template, char startSymbol = '{', char endSymbol = '}')
    {
        this.template = template ?? string.Empty;
        this.startSymbol = startSymbol;
        this.endSymbol = endSymbol;
        ValidateTemplate();
    }

    [Category("Данные")]
    [Description("Заполняет список объектами по шаблону.")]
    public void FillItems<T>(IEnumerable<T> objects)
    {
        if (string.IsNullOrEmpty(template))
            throw new TemplateFormatException("Сначала задайте шаблон через SetTemplate().");

        var tokens = ExtractTokens(template, startSymbol, endSymbol).ToArray();

        listBox.BeginUpdate();
        try
        {
            listBox.Items.Clear();
            foreach (var obj in objects)
            {
                var line = ApplyTemplate(template, obj!, tokens, startSymbol, endSymbol);
                listBox.Items.Add(new ItemWrap(line, obj!));
            }
        }
        finally { listBox.EndUpdate(); }
    }

    [Category("Данные")]
    public void ClearItems() => listBox.Items.Clear();

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public T? GetSelectedObject<T>() where T : new()
    {
        if (listBox.SelectedItem is not ItemWrap wrap) return default;

        // Создаём НОВЫЙ экземпляр и копируем публичные свойства/поля
        var dst = new T();
        CopyPublicMembers(wrap.Source, dst);
        return dst;
    }

    private static IEnumerable<string> ExtractTokens(string tpl, char open, char close)
    {
        var tokens = new List<string>();
        var sb = new StringBuilder();
        bool inside = false;
        foreach (var ch in tpl)
        {
            if (!inside && ch == open) { inside = true; sb.Clear(); continue; }
            if (inside && ch == close) { inside = false; var t = sb.ToString(); if (!string.IsNullOrWhiteSpace(t)) tokens.Add(t); continue; }
            if (inside) sb.Append(ch);
        }
        return tokens.Distinct();
    }

    private void ValidateTemplate()
    {
        if (string.IsNullOrWhiteSpace(template))
            throw new TemplateFormatException("Пустой шаблон.");

        // Запрет начинать свойством (оканчиваться свойством — разрешаем)
        if (template[0] == startSymbol)
            throw new TemplateFormatException("Шаблон не должен начинаться со свойства.");

        bool lastWasToken = false;
        int i = 0;
        while (i < template.Length)
        {
            if (template[i] == startSymbol)
            {
                int j = template.IndexOf(endSymbol, i + 1);
                if (j < 0)
                    throw new TemplateFormatException("Несбалансированные скобки в шаблоне.");
                if (j == i + 1)
                    throw new TemplateFormatException("Пустое имя свойства в шаблоне.");
                if (lastWasToken)
                    throw new TemplateFormatException("В шаблоне не должно быть двух свойств подряд без текста между ними.");
                lastWasToken = true;
                i = j + 1;
            }
            else
            {
                lastWasToken = false;
                i++;
            }
        }
    }

    private static string ApplyTemplate<T>(string tpl, T obj, IEnumerable<string> tokens, char open, char close)
    {
        string line = tpl;
        var type = obj!.GetType();
        foreach (var t in tokens)
        {
            var mem = type.GetMember(t, BindingFlags.Public | BindingFlags.Instance).FirstOrDefault();
            object? val = null;
            if (mem is PropertyInfo pi) val = pi.GetValue(obj);
            else if (mem is FieldInfo fi) val = fi.GetValue(obj);
            line = line.Replace($"{open}{t}{close}", val?.ToString() ?? string.Empty);
        }
        return line;
    }

    private static void CopyPublicMembers(object source, object dest)
    {
        var sType = source.GetType();
        var dType = dest.GetType();

        foreach (var sp in sType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var dp = dType.GetProperty(sp.Name, BindingFlags.Public | BindingFlags.Instance);
            if (dp?.CanWrite == true)
            {
                var val = sp.GetValue(source);
                dp.SetValue(dest, val);
            }
        }

        foreach (var sf in sType.GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            var df = dType.GetField(sf.Name, BindingFlags.Public | BindingFlags.Instance);
            if (df != null)
            {
                var val = sf.GetValue(source);
                df.SetValue(dest, val);
            }
        }
    }
}
