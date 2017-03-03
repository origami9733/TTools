using System;
using System.ComponentModel;
using TTools.Domain;

namespace TTools.Models
{
    public class SideMenuItem : INotifyPropertyChanged
    {
        public SideMenuItem(string name, object content)
        {
            _name = name;
            Content = content;
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { this.MutateVerbose(ref _name, value, RaisePropertyChanged()); }
        }

        private object _content;
        public object Content
        {
            get { return _content; }
            set { this.MutateVerbose(ref _content, value, RaisePropertyChanged()); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
