using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TTools.Domain;
using TTools.EF;
using TTools.Models;
using Microsoft.VisualBasic;
using Smart.Text.Japanese;
using System.Diagnostics;
using System.Windows.Input;

namespace TTools.ViewModels
{
    public class EItemSelectDialogVM : INotifyPropertyChanged
    {
        private TechnoDB context;
        private ICollectionView collectionView;

        private DispatchObservableCollection<EItem> _eItems;
        public DispatchObservableCollection<EItem> EItems
        {
            get { return _eItems; }
            set
            {
                if (_eItems == value) return;
                _eItems = value;
                RaisePropertyChanged();
            }
        }

        private EItem _selectedEItem;
        public EItem SelectedEitem
        {
            get { return _selectedEItem; }
            set
            {
                if (_selectedEItem == value) return;
                _selectedEItem = value;
                RaisePropertyChanged();
            }
        }

        private string _searchString;
        public string SearchString
        {
            get { return _searchString; }
            set
            {
                if (_searchString == value) return;
                _searchString = value;
                RaisePropertyChanged();
                SetFilter();
            }
        }

        public EItemSelectDialogVM()
        {
            Reload();
        }

        private async void Reload()
        {
            await Task.Run(() =>
            {
                context = new TechnoDB();
                EItems = new DispatchObservableCollection<EItem>(context.EItems);
            });

            collectionView = CollectionViewSource.GetDefaultView(EItems);
        }

        private ICommand _eItemAddCommand;
        public ICommand EItemAddCommand
        {
            get
            {
                if (_eItemAddCommand != null) return _eItemAddCommand;
                _eItemAddCommand = new RelayCommand<object>(ExecuteEItemAddCommand);
                return _eItemAddCommand;
            }
        }
        public void ExecuteEItemAddCommand(object arg)
        {
            string id;
            using (TechnoDB newContext = new TechnoDB())
            {
                id = newContext.EItems.Select(x => x.ID).Max();
            }
            int id2 = int.Parse(id);
            id2++;
            id = id2.ToString();

            string add0 = null;

            for (int i = 0; i < (5 - id.Length); i++)
            {
                add0 = "0" + add0;
            }

            var item = new EItem();
            item.ID = add0 + id;
            context.EItems.Add(item);
            SearchString = null;
            context.SaveChanges();
            Reload();
        }


        private void SetFilter()
        {
            if (String.IsNullOrEmpty(SearchString) == true || string.IsNullOrWhiteSpace(SearchString) == true)
            {
                collectionView.Filter = null;
            }
            else
            {
                collectionView.Filter += x =>
                {
                    bool accept = false;
                    var item = (EItem)x;
                    List<string> strArray = new List<string>();
                    List<string> dummyArray = null;

                    if (SearchString != null)
                    {
                        dummyArray = OriginalStringConverter.SearchWordConvert(SearchString).Split(new char[] { ' ', '　' }).ToList();
                        //Nullと空と空白をリムーブ
                        foreach (var a in dummyArray)
                        {
                            if (string.IsNullOrEmpty(a) != true || string.IsNullOrWhiteSpace(a) != true)
                            {
                                strArray.Add(a);
                            }
                        }

                        foreach (var searchStr in strArray)
                        {
                            foreach (var a in item)
                            {
                                if (a != null)
                                {
                                    if (OriginalStringConverter.SearchWordConvert(a.ToString()).Contains(searchStr))
                                    {
                                        accept = true;
                                        break;
                                    }
                                    else
                                    {
                                        accept = false;
                                    }
                                }
                            }
                            if (accept == false) break;
                        }
                    }
                    return accept;
                };
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
