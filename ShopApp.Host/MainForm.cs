using ShopApp.Components;

namespace ShopApp.Host;

public partial class MainForm : Form
{
    private readonly MenuStrip menu = new();
    private readonly ToolStripMenuItem miDirectories = new("Справочники");
    private readonly ToolStripMenuItem miReports = new("Отчёты");
    private readonly TabControl tabs = new() { Dock = DockStyle.Fill };

    public MainForm()
    {
        InitializeComponent();

        Text = "Учёт заказов (Вариант 19) — ЛР2 'Хорошо'";
        Width = 900; Height = 600;

        menu.Items.AddRange(new ToolStripItem[] { miDirectories, miReports });
        MainMenuStrip = menu;
        Controls.Add(tabs);
        Controls.Add(menu);

        // Меню статически (уровень «Хорошо»)
        miDirectories.DropDownItems.Add("Статусы заказа", null, (_, __) => OpenTab("Статусы заказа", () => new StatusDirectoryControl()));
        miDirectories.DropDownItems.Add("Заказы", null, (_, __) => OpenTab("Заказы", () => new OrdersControl()));
        miReports.DropDownItems.Add("Отчёт по заказам", null, (_, __) => OpenTab("Отчёт по заказам", () => new OrdersReportControl()));

        // Закрытие вкладки по двойному клику
        tabs.MouseDoubleClick += (_, __) =>
        {
            if (tabs.TabPages.Count == 0) return;
            var page = tabs.SelectedTab;
            if (page != null) tabs.TabPages.Remove(page);
        };
    }

    private void OpenTab(string title, Func<Control> factory)
    {
        foreach (TabPage p in tabs.TabPages)
            if (p.Text == title) { tabs.SelectedTab = p; return; }

        var page = new TabPage(title) { Padding = new Padding(0) };
        var ctl = factory(); ctl.Dock = DockStyle.Fill;
        page.Controls.Add(ctl);
        tabs.TabPages.Add(page);
        tabs.SelectedTab = page;
    }
}
