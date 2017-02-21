using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace TTools.Models
{
    public class ProductItem : INotifyPropertyChanged
    {
        private string _Id;
        private string _longName;
        private string _shortName;
        private string _aliasName;
        private float _price;
        private string _category;
        private string _detail;
        private string _bluePrint;
        private int _leadTime;

        [Key]
        public string ProductId
        {
            get { return _Id; }
            set
            {
                if (_Id == value) return;
                _Id = value;
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
        public float Price
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
        public int LeadTime
        {
            get { return _leadTime; }
            set
            {
                if (_leadTime == value) return;
                _leadTime = value;
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
