using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace TTools.Models
{
    public class RelationItem : INotifyPropertyChanged
    {
        private string _productId;
        private string _eItemId;
        private string _amount;

        [Key]
        [Column(Order = 0)]
        public string ProductId
        {
            get { return _productId; }
            set
            {
                if (_productId == value) return;
                _productId = value;
                RaisePropertyChanged();
            }
        }
        [Key]
        [Column(Order = 1)]
        public string EItemId
        {
            get { return _eItemId; }
            set
            {
                if (_eItemId == value) return;
                _eItemId = value;
                RaisePropertyChanged();
            }
        }
        public string Amount
        {
            get { return _amount; }
            set
            {
                if (_amount == value) return;
                _amount = value;
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
