using System.ComponentModel;
using System.Runtime.CompilerServices;
using TTools.Models;
using TTools.Views;

namespace TTools.ViewModels
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowVM()
        {
            SideMenuItems = new[]
            {
                new SideMenuItem("インフォメーション",new Information()),
                new SideMenuItem("受注インポート",new ImportOrder(){DataContext = new ImportOrderVM(this) }),
                new SideMenuItem("BS受注管理",new BsOrdersManagement(){DataContext = new BsOrderManagementVM(this) }),
                new SideMenuItem("BS回答管理",new Views.BsReplyManagement(){DataContext = new BsReplyManagementVM(this) }),
            };
        }


        public SideMenuItem[] SideMenuItems { get; }

        private int _selectedSideMenuIndex = 1;
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


        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
