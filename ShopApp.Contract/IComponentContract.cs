using System.Windows.Forms;

namespace ShopApp.Contract;

public interface IComponentContract
{
    string Id { get; }                 // уникальный ID компонента
    string Title { get; }              // текст в меню/вкладке
    string Category { get; }           // "Справочники" или "Отчёты"
    UserControl CreateControl();       // фабрика контрола
}
