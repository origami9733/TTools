using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using TTools.Models;
using TTools.Views;

namespace TTools.ViewModels
{
    public class MainWindowVM:INotifyPropertyChanged
    {
        public SideMenuItem[] SideMenuItems { get; }

        private int _selectedSideMenuIndex = 2;
        public int SelectedSideMenuIndex
        {
            get { return _selectedSideMenuIndex; }
            set
            {
                if (_selectedSideMenuIndex == value) return;
                _selectedSideMenuIndex = value;
                RaisePropertyChanged();
            }
        }
        

        public MainWindowVM()
        {
        
            SideMenuItems = new[]
            {
                new SideMenuItem("インフォメーション",new Information()),
                new SideMenuItem("受注インポート",new ImportOrders(){}),
                new SideMenuItem("BS受注管理",new OrdersManagement(){DataContext = new BsOrderManagementVM(this) }),
                new SideMenuItem("BS回答管理",new OrderEdit(){DataContext = new OrderEditVM(this) }),

            };
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
