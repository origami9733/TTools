using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TTools.Models
{
    public class EItem : INotifyPropertyChanged,IEnumerable
    {
        private string _id;
        private string _name;
        private string _model;
        private string _price;
        private string _category;
        private string _vendorId;

        [Key]
        public string ID
        {
            get { return _id; }
            set
            {
                if (_id == value) return;
                _id = value;
                RaisePropertyChanged();
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                _name = value;
                RaisePropertyChanged();
            }
        }
        public string Model
        {
            get { return _model; }
            set
            {
                if (_model == value) return;
                _model = value;
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
        public string Vender
        {
            get { return _vendorId; }
            set
            {
                if (_vendorId == value) return;
                _vendorId = value;
                RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// フィルター条件を指定する際にforeachでプロパティを回す為。
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            yield return ID;
            yield return Name;
            yield return Model;
            yield return Price;
            yield return Vender;
        }

        public void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
