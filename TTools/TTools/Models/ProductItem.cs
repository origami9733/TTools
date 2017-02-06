using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace TTools.Models
{
    public class ProductItem : INotifyPropertyChanged
    {
        #region 変数
        private string _productId;
        private string _longName;
        private string _shortName;
        private string _aliasName;
        private string _price;
        private string _category;
        #endregion

        #region プロパティ
        [Key]
        public string ProductId
        {
            get { return _productId; }
            set
            {
                if (_productId != value)
                {
                    _productId = value;
                }
                RaisePropertyChanged();
            }
        }
        public string LongName
        {
            get
            { return _longName; }
            set
            {
                if (_longName != value)
                {
                    _longName = value;
                }
                RaisePropertyChanged();
            }
        }
        public string ShortName
        {
            get { return _shortName; }
            set
            {
                if (_shortName != value)
                {
                    _shortName = value;
                }
                RaisePropertyChanged();
            }
        }
        public string AliasName
        {
            get { return _aliasName; }
            set
            {
                if (_aliasName != value)
                {
                    _aliasName = value;
                }
                RaisePropertyChanged();
            }
        }
        public string Price
        {
            get { return _price; }
            set
            {
                if (_price != value)
                {
                    _price = value;
                }
                RaisePropertyChanged();
            }
        }
        public string Category
        {
            get { return _category; }
            set
            {
                if (_category != value)
                {
                    _category = value;
                }
                RaisePropertyChanged();
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
