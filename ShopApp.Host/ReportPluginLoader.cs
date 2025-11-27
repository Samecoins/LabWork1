using ReportContracts;
using System.Reflection;

namespace ShopApp.Host;
internal static class ReportPluginLoader
{
    private static IReadOnlyList<T> LoadPlugins<T>(string dir)
    {
        var result = new List<T>();
        Directory.CreateDirectory(dir);

        foreach (var dll in Directory.GetFiles(dir, "*.dll"))
        {
            Assembly asm;
            try
            {
                asm = Assembly.LoadFrom(dll);
            }
            catch
            {
                continue;
            }

            var types = asm.GetTypes()
                .Where(t => !t.IsAbstract && typeof(T).IsAssignableFrom(t));

            foreach (var t in types)
            {
                try
                {
                    if (Activator.CreateInstance(t) is T plugin)
                        result.Add(plugin);
                }
                catch
                {
                    // пропускаем битые
                }
            }
        }

        return result;
    }

    public static IReadOnlyList<IReportDocumentWithContextTextsContract> LoadTextPlugins(string dir) =>
        LoadPlugins<IReportDocumentWithContextTextsContract>(dir);

    public static IReadOnlyList<IReportDocumentWithChartPieContract> LoadPiePlugins(string dir) =>
        LoadPlugins<IReportDocumentWithChartPieContract>(dir);

    public static IReadOnlyList<IReportDocumentWithTableColumnRowHeaderContract> LoadTablePlugins(string dir) =>
        LoadPlugins<IReportDocumentWithTableColumnRowHeaderContract>(dir);
}
