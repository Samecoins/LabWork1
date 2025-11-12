using ShopApp.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApp.Components;

public sealed class StatusDirectoryComponent : IComponentContract
{
    public string Id => "status-directory";
    public string Title => "Статусы заказа";
    public string Category => "Справочники";
    public UserControl CreateControl() => new StatusDirectoryControl();
}
