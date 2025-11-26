using System.Windows.Forms;

namespace ShopApp.Contract;

public interface IComponentContract
{
    string Id { get; }               
    string Title { get; }             
    string Category { get; }          
    UserControl CreateControl();      
}
