using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace TTools.Models
{
    public class ProductItem : INotifyPropertyChanged
    {
        private string _productId;
        private string _longName;
        private string _shortName;
        private string _aliasName;
        private string _price;
        private string _category;
        private string _detail;
        private string _bluePrint;

        [Key]
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
        public string LongName
        {
            get { return _longName; }
            set
            {
                if (_longName == value) return;
                _longName = value;
                RaisePropertyChanged();
            }
        }
        public string ShortName
        {
            get { return _shortName; }
            set
            {
                if (_shortName == value) return;
                _shortName = value;
                RaisePropertyChanged();
            }
        }
        public string AliasName
        {
            get { return _aliasName; }
            set
            {
                if (_aliasName == value) return;
                _aliasName = value;
                RaisePropertyChanged();
            }
        }
        public string Price
        {
            get { return _price; }
            set
            {
                if (_price == value) return;
                _price = value;
                RaisePropertyChanged();
            }
        }
        public string Category
        {
            get { return _category; }
            set
            {
                if (_category == value) return;
                _category = value;
                RaisePropertyChanged();
            }
        }
        public string Detail
        {
            get { return _detail; }
            set
            {
                if (_detail == value) return;
                _detail = value;
                RaisePropertyChanged();
            }
        }
        public string BluePrint
        {
            get { return _bluePrint; }
            set
            {
                if (_bluePrint == value) return;
                _bluePrint = value;
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
