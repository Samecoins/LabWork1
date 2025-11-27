using ShopApp.Contract;
using System.Reflection;
using System.Text.Json;

namespace ShopApp.Host;

public partial class MainForm : Form
{
    private readonly MenuStrip menu = new();
    private readonly ToolStripMenuItem miDirectories = new("Справочники");
    private readonly ToolStripMenuItem miReports = new("Отчёты");
    private readonly ToolStripMenuItem miExtensions = new("Расширения");
    private readonly TabControl tabs = new() { Dock = DockStyle.Fill };

    public MainForm()
    {
        InitializeComponent();

        Text = "Учёт заказов (Вариант 19) — ЛР2 'Хорошо'";
        Width = 900; Height = 600;

        menu.Items.AddRange(new ToolStripItem[] { miDirectories, miReports, miExtensions});

        miExtensions.Click += (_, __) =>
        {
            using var frm = new ExtensionsForm();
            frm.ShowDialog(this);
        };

        MainMenuStrip = menu;
        Controls.Add(tabs);
        Controls.Add(menu);

        tabs.MouseDoubleClick += (_, __) =>
        {
            var page = tabs.SelectedTab;
            if (page != null) tabs.TabPages.Remove(page);
        };

        try
        {
            var all = LoadExtensions();
            var allowed = FilterByLicense(all);

            foreach (var c in allowed.Where(x => x.Category == "Справочники"))
            {
                var mi = new ToolStripMenuItem(c.Title);
                mi.Click += (_, __) => OpenTab(c);
                miDirectories.DropDownItems.Add(mi);
            }
            foreach (var c in allowed.Where(x => x.Category == "Отчёты"))
            {
                var mi = new ToolStripMenuItem(c.Title);
                mi.Click += (_, __) => OpenTab(c);
                miReports.DropDownItems.Add(mi);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка при загрузке компонентов");
        }
    }

    private void OpenTab(IComponentContract comp)
    {
        foreach (TabPage p in tabs.TabPages)
            if (p.Text == comp.Title) { tabs.SelectedTab = p; return; }

        var page = new TabPage(comp.Title) { Padding = new Padding(0) };
        var ctl = comp.CreateControl(); ctl.Dock = DockStyle.Fill;
        page.Controls.Add(ctl);
        tabs.TabPages.Add(page);
        tabs.SelectedTab = page;
    }

    private static List<IComponentContract> LoadExtensions()
    {
        var list = new List<IComponentContract>();
        var dir = Path.Combine(AppContext.BaseDirectory, "Components");
        Directory.CreateDirectory(dir);

        foreach (var dll in Directory.GetFiles(dir, "*.dll"))
        {
            try
            {
                var asm = Assembly.LoadFrom(dll);
                var types = asm.GetTypes()
                    .Where(t => typeof(IComponentContract).IsAssignableFrom(t) && !t.IsAbstract);

                foreach (var t in types)
                    if (Activator.CreateInstance(t) is IComponentContract comp)
                        list.Add(comp);
            }
            catch { /* пропускаем неподходящие */ }
        }
        return list;
    }

    private static IReadOnlyList<IComponentContract> FilterByLicense(IEnumerable<IComponentContract> comps)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "license.json");
        if (!File.Exists(path)) return comps.ToList();

        var json = File.ReadAllText(path);

        var opts = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var lic = JsonSerializer.Deserialize<LicenseModel>(json, opts) ?? new();

        MessageBox.Show($"Role: {lic.Role}\nAllow count: {lic.Allow?.Count ?? 0}");

        if (lic.Allow != null && lic.Allow.Count > 0)
            return comps.Where(c => lic.Allow.Contains(c.Id, StringComparer.OrdinalIgnoreCase)).ToList();

        return lic.Role?.ToLowerInvariant() switch
        {
            "minimal" => comps.Where(c => c.Id == "status-directory").ToList(),
            "basic" => comps.Where(c => c.Id is "status-directory" or "orders").ToList(),
            "advanced" => comps.ToList(),
            _ => comps.ToList()
        };
    }


    private sealed class LicenseModel
    {
        public string? Role { get; set; }
        public List<string> Allow { get; set; } = new();
    }
}
