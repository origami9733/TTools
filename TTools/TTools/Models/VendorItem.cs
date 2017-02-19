using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TTools.Models
{
    public class VendorItem : INotifyPropertyChanged
    {
        private string _id;
        private string _mail;
        private string _adr;
        private string _zip;
        private string _fax;
        private string _tel;
        private string _name;

        public string Id
        {
            get { return _id; }
            set
            {
                if (_id == value) return;
                _id = value;
                OnPropertyChanged();
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged();
            }
        }
        public string Tel
        {
            get { return _tel; }
            set
            {
                if (_tel == value) return;
                _tel = value;
                OnPropertyChanged();
            }
        }
        public string Fax
        {
            get { return _fax; }
            set
            {
                if (_fax == value) return;
                _fax = value;
                OnPropertyChanged();
            }
        }
        public string Zip
        {
            get { return _zip; }
            set
            {
                if (_zip == value) return;
                _zip = value;
                OnPropertyChanged();
            }
        }
        public string Adr
        {
            get { return _adr; }
            set
            {
                if (_adr == value) return;
                _adr = value;
                OnPropertyChanged();
            }
        }
        public string Mail
        {
            get { return _mail; }
            set
            {
                if (_mail == value) return;
                _mail = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
