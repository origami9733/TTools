using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using TTools.Domain;
using TTools.EF;
using TTools.Models;

namespace TTools.ViewModels
{
    public class EItemSelectDialogVM : INotifyPropertyChanged
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EItemSelectDialogVM()
        {
            Load();
        }

        #region ローカル変数
        private TechnoDB context;
        private ICollectionView collectionView;
        #endregion

        #region プロパティ
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
        public EItem SelectedEItem
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
        #endregion

        /// <summary>
        /// ロード
        /// </summary>
        private async void Load()
        {
            await Task.Run(() =>
            {
                context = new TechnoDB();
                EItems = new DispatchObservableCollection<EItem>(context.EItems);
            });

            collectionView = CollectionViewSource.GetDefaultView(EItems);
        }

        /// <summary>
        /// 自動採番した空のアイテムを追加する。
        /// </summary>
        private void AddNewEItem()
        {
            using (TechnoDB newContext = new TechnoDB())
            {
                string originalValue = newContext.EItems.Select(x => x.Id).Max();
                int oldValue;
                string newValue;
                string fillStr;

                if (originalValue == null)
                {
                    fillStr = "0000";
                    newValue = "1";
                }
                else
                {
                    oldValue = int.Parse(originalValue);
                    oldValue++;
                    newValue = oldValue.ToString();

                    fillStr = null;

                    for (int i = 0; i < (5 - newValue.Length); i++)
                    {
                        fillStr = "0" + fillStr;
                    }
                }

                var addItem = new EItem();
                addItem.Id = fillStr + newValue;

                context.EItems.Add(addItem);
                context.SaveChanges();

                SearchString = null;
                Load();
            }
        }

        /// <summary>
        /// リアルタイムフィルター
        /// </summary>
        private void SetFilter()
        {
            if (string.IsNullOrEmpty(SearchString) == true || string.IsNullOrWhiteSpace(SearchString) == true)
            {
                collectionView.Filter = null;
            }
            else
            {
                collectionView.Filter += x =>
                {
                    bool accept = false;
                    var eItem = (EItem)x;

                    List<string> searchWordArray = new List<string>();
                    List<string> dummySearchWordArray = null;

                    if (SearchString != null)
                    {
                        //検索ワードを空白でスプリットしてダミー配列に格納
                        dummySearchWordArray = OriginalStringConverter.SearchStringConvert(SearchString).Split(new char[] { ' ', '　' }).ToList();
                        
                        //ダミー配列から無駄な要素以外を正規配列に格納
                        foreach (var a in dummySearchWordArray)
                        {
                            if (string.IsNullOrEmpty(a) != true || string.IsNullOrWhiteSpace(a) != true)
                            {
                                searchWordArray.Add(a);
                            }
                        }

                        //検索ワード配列の文字列を含むアイテムのみ返す
                        foreach (var searchWord in searchWordArray)
                        {
                            foreach (var a in eItem)
                            {
                                if (a != null)
                                {
                                    if (OriginalStringConverter.SearchStringConvert(a.ToString()).Contains(searchWord))
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
            AddNewEItem();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
