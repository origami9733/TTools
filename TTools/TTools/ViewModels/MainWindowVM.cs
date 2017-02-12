using TTools.Models;
using TTools.Views;

namespace TTools.ViewModels
{
    public class MainWindowVM
    {
        public SideMenuItem[] SideMenuItems { get; }
        public ImportOrdersVM importOrdersVM;

        public MainWindowVM()
        {
            importOrdersVM = new ImportOrdersVM();

            SideMenuItems = new[]
            {
                new SideMenuItem("インフォメーション",new Information()),
                new SideMenuItem("受注インポート",new ImportOrders(){ DataContext = importOrdersVM }),
                new SideMenuItem("ＢＳ受注管理",new OrdersManagement(){ DataContext = new BsOrderManagementVM()}),
            };
        }
    }
}
