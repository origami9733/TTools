using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TTools.Models
{
    public class DisplayReplyManagementItem : INotifyPropertyChanged
    {
        private OrderItem _orderItem;
        private ProductItem _productItem;
        private RelationItem _relationItem;
        private EItem _eItem;
        private VendorItem _vendorItem;
        private string _replyDate;

        public OrderItem OrderItem
        {
            get { return _orderItem; }
            set
            {
                if (_orderItem == value) return;
                _orderItem = value;
                RaisePropertyChanged();
            }
        }
        public ProductItem ProductItem
        {
            get { return _productItem; }
            set
            {
                if (_productItem == value) return;
                _productItem = value;
                RaisePropertyChanged();
            }
        }
        public RelationItem RelationItem
        {
            get { return _relationItem; }
            set
            {
                if (_relationItem == value) return;
                _relationItem = value;
                RaisePropertyChanged();
            }
        }
        public EItem EItem
        {
            get { return _eItem; }
            set
            {
                if (_eItem == value) return;
                _eItem = value;
                RaisePropertyChanged();
            }
        }
        public VendorItem VendorItem
        {
            get { return _vendorItem; }
            set
            {
                if (_vendorItem == value) return;
                _vendorItem = value;
                RaisePropertyChanged();
            }
        }
        public string ReplyDate
        {
            get { return _replyDate; }
            set
            {
                if (_replyDate == value) return;
                if(value.Length > 10)
                {
                    _replyDate = value.Substring(0,10);
                }
                else
                {
                    _replyDate = value;
                }
                
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
