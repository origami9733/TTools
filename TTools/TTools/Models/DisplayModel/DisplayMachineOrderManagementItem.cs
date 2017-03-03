using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TTools.Models
{
    public class DisplayMachineOrderManagementItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private OrderItem _orderItem;
        public OrderItem OrderItem
        {
            get { return _orderItem; }
            set
            {
                if (_orderItem == value) return;
                _orderItem = value;
                RaisePropertyChanged();
                RaisePropertyChanged(OrderId);
            }
        }

        private ProductItem _productItem;
        public ProductItem ProductItem
        {
            get { return _productItem; }
            set
            {
                if (_productItem == value) return;
                _productItem = value;
                RaisePropertyChanged();
                RaisePropertyChanged(ShortName);
            }
        }
        
        public string OrderId
        {
            get { return OrderItem.伝票ＮＯ.Substring(0,10); }
        }
        public string ShortName
        {
            get { return ProductItem.ShortName; }
        }

        private string _sumPrice;
        public string SumPrice
        {
            get { return _sumPrice; }
            set
            {
                if (_sumPrice == value) return;
                _sumPrice = value;
                RaisePropertyChanged();
            }
        }

        private string _itemsCount;
        public string ItemsCount
        {
            get { return _itemsCount; }
            set
            {
                if (_itemsCount == value) return;
                _itemsCount = value;
                RaisePropertyChanged();
            }
        }
        
    }
}
