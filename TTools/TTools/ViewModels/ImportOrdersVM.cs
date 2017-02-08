using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TTools.EF;
using TTools.Models;
using TTools.Views;
using TTools.Domain;

namespace TTools.ViewModels
{
    public class ImportOrdersVM : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region Validation
        #region INotigyPropertyChangedインターフェース
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region INotifyDataErrorInfoインターフェース

        #region 発生中のエラーを保持する処理を実装
        readonly Dictionary<string, string> _currentErrors = new Dictionary<string, string>();

        protected void AddError(string propertyName, string error)
        {
            if (!_currentErrors.ContainsKey(propertyName))
                _currentErrors[propertyName] = error;

            OnErrorsChanged(propertyName);
        }

        protected void RemoveError(string propertyName, string error)
        {
            if (_currentErrors.ContainsKey(propertyName))
                _currentErrors.Remove(propertyName);

            OnErrorsChanged(propertyName);
        }
        #endregion

        private void OnErrorsChanged(string propertyName)
        {
            var h = this.ErrorsChanged;
            if (h != null)
            {
                h(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        #region INotifyDataErrorInfoの実装
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) ||
                !_currentErrors.ContainsKey(propertyName))
                return null;

            return _currentErrors[propertyName];
        }

        public bool HasErrors
        {
            get { return _currentErrors.Count > 0; }
        }
        #endregion
        #endregion


        //エラー検証
        protected void ValidateProperty(string propertyName, object value)
        {
            switch (propertyName)
            {
                case nameof(UpdateDate):
                    if (String.IsNullOrEmpty(SendedFlag) == false && this.UpdateDate == null)
                        AddError(nameof(UpdateDate), "aaaaa");
                    else
                        RemoveError(nameof(UpdateDate), null);
                    break;
                default:
                    break;
            }
        }

        #endregion

        public ImportOrdersVM()
        {
            OpenDialogCommand = new AnotherCommandImplementation(OpenDialog);
        }

        #region 検索条件プロパティ

        //伝送フラグ
        private string _sendedFlag = string.Empty;
        public string SendedFlag
        {
            get { return _sendedFlag; }
            set
            {
                if (_sendedFlag != value)
                    _sendedFlag = value;
                ValidateProperty(nameof(UpdateDate), value);
            }
        }


        //更新日付
        private DateTime? _updateDate = null;
        public DateTime? UpdateDate
        {
            get { return _updateDate; }
            set
            {
                if (_updateDate != value)
                    _updateDate = value;
                ValidateProperty(nameof(UpdateDate), value);
            }
        }
        
        #endregion

     
        //EFインスタンス
        private TechnoDB context = new TechnoDB();


        //OrderItemデータロード用
        private OrderItem orderItem = new OrderItem();


        //コレクションビュー
        private ICollectionView collectionView;


        //選択中カテゴリー
        private string _selectedCategory;
        public string SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    RaisePropertyChanged();
                }
            }
        }


        //選択中アイテム
        private OrderItem _selectedRowItem;
        public OrderItem SelectedRowItem
        {
            get { return _selectedRowItem; }
            set
            {
                if (_selectedRowItem != value)
                {
                    _selectedRowItem = value;
                    RaisePropertyChanged();
                }
            }
        }


        //オリジナルの受注コレクション
        private DispatchObservableCollection<OrderItem> _originalOrderItems;
        public DispatchObservableCollection<OrderItem> OriginalOrderItems
        {
            get { return _originalOrderItems; }
            set
            {
                if (_originalOrderItems != value)
                {
                    _originalOrderItems = value;
                    RaisePropertyChanged();
                }
            }
        }


        //表示用の受注コレクション
        private DispatchObservableCollection<SelectableOrderItem> _displayOrderItems;
        public DispatchObservableCollection<SelectableOrderItem> DisplayOrderItems
        {
            get { return _displayOrderItems; }
            set
            {
                if (_displayOrderItems != value)
                {
                    _displayOrderItems = value;
                    RaisePropertyChanged();
                }
            }
        }


        //チェックボックスカラムのヘッダのチェック状態
        private bool _checkBoxColumnIsChecked = false;
        public bool CheckBoxColumnIsChecked
        {
            get { return _checkBoxColumnIsChecked; }
            set
            {
                _checkBoxColumnIsChecked = value;
                RaisePropertyChanged();
            }
        }


        //カテゴリー毎のアイテム数カウンター
        private int? _allCategoryCount;
        private int? _machineCategoryCount;
        private int? _bsCategoryCount;
        private int? _plCategoryCount;
        public int? AllCategoryCount
        {
            get { return _allCategoryCount; }
            set
            {
                if (_allCategoryCount != value)
                {
                    _allCategoryCount = value;
                    RaisePropertyChanged();
                }
            }
        }
        public int? MachineCategoryCount
        {
            get { return _machineCategoryCount; }
            set
            {
                if (_machineCategoryCount != value)
                {
                    _machineCategoryCount = value;
                    RaisePropertyChanged();
                }
            }
        }
        public int? BsCategoryCount
        {
            get { return _bsCategoryCount; }
            set
            {
                if (_bsCategoryCount != value)
                {
                    _bsCategoryCount = value;
                    RaisePropertyChanged();
                }
            }
        }
        public int? PlCategoryCount
        {
            get { return _plCategoryCount; }
            set
            {
                if (_plCategoryCount != value)
                {
                    _plCategoryCount = value;
                    RaisePropertyChanged();
                }
            }
        }


        //カウンター群のリロード
        private void ReloadCounters()
        {
            if (DisplayOrderItems?.Count() != null)
                AllCategoryCount = DisplayOrderItems.Count();
            else
                AllCategoryCount = null;


            if (DisplayOrderItems?.Where(x => x.OrderItem.フラグ１ == " ").Count() != null)
                MachineCategoryCount = DisplayOrderItems.Where(x => x.OrderItem.フラグ１ == " ").Count();
            else
                MachineCategoryCount = null;


            if (DisplayOrderItems?.Where(x => x.OrderItem.フラグ１ == "0").Count() != null)
                BsCategoryCount = DisplayOrderItems?.Where(x => x.OrderItem.フラグ１ == "0").Count();
            else
                BsCategoryCount = null;


            if (DisplayOrderItems?.Where(x => x.OrderItem.フラグ１ == "1").Count() != null)
                PlCategoryCount = DisplayOrderItems?.Where(x => x.OrderItem.フラグ１ == "1").Count();
            else
                PlCategoryCount = null;
        }

        


        //SQL文章の作成
        private string MakeSQL()
        {
            string sql = "SELECT * FROM dbo.GH_0000101 WHERE 伝送フラグ = ";

            if (SendedFlag == "")
                sql = sql + "0" + " ";
            else
                sql = string.Format(sql + "{0} ", SendedFlag);

            if (UpdateDate != null)
                sql = string.Format(sql + "AND 更新日付 >= '{0}'", UpdateDate.Value.ToString("yyyy/MM/dd"));

            Debug.Print("★SQL発行： " + sql);

            return sql;
        }



        //データリロード
        private ICommand _reloadCommand;
        public ICommand ReloadCommand
        {
            get
            {
                if (_reloadCommand == null)
                {
                    _reloadCommand = new RelayCommand<string>(ExecuteReloadCommand, CanExecuteReloadCommand);
                }
                return _reloadCommand;
            }
        }
        private bool CanExecuteReloadCommand(string arg)
        {
            if (_currentErrors.ContainsKey(nameof(UpdateDate)) == true)
                return false;

            return true;
        }
        private void ExecuteReloadCommand(String arg)
        {
        }





        //カテゴリー切り替えコマンド
        private ICommand _categoryChangeCommand;
        public ICommand CategoryChangeCommand
        {
            get
            {
                if (_categoryChangeCommand == null)
                {
                    _categoryChangeCommand = new RelayCommand<string>(ExecuteCategoryChangeCommand, CanExecuteCategoryChangeCommand);
                }
                return _categoryChangeCommand;
            }
        }
        private bool CanExecuteCategoryChangeCommand(string msg)
        {
            return OriginalOrderItems != null;
        }
        private void ExecuteCategoryChangeCommand(string msg)
        {
            if (SelectedCategory != msg)
            {
                switch (msg)
                {
                    case "ShowAll":
                        collectionView.Filter = null;
                        UnCheckAll();
                        break;
                    case "Machine":
                        collectionView.Filter += x =>
                        {
                            var item = (SelectableOrderItem)x;
                            return item.OrderItem.フラグ１ == " ";
                        };
                        UnCheckAll();
                        break;
                    case "BS":
                        collectionView.Filter += x =>
                        {
                            var item = (SelectableOrderItem)x;
                            return item.OrderItem.フラグ１ == "0";
                        };
                        UnCheckAll();
                        break;
                    case "PL":
                        collectionView.Filter = x =>
                        {
                            var item = (SelectableOrderItem)x;
                            return item.OrderItem.フラグ１ == "1";
                        };
                        UnCheckAll();
                        break;
                }
            }
        }


        //チェックボックスリセット
        private void UnCheckAll()
        {
            foreach (var a in DisplayOrderItems)
            {
                a.IsChecked = false;
            }
            CheckBoxColumnIsChecked = false;
        }


        //アンロードコマンド
        private ICommand _unloadCommand;
        public ICommand UnloadCommand
        {
            get
            {
                if (_unloadCommand == null)
                {
                    _unloadCommand = new RelayCommand<string>(ExecuteUnloadCommand, CanExecuteChangeUnloadCommand);
                }
                return _unloadCommand;
            }
        }
        public bool CanExecuteChangeUnloadCommand(string msg)
        {
            return DisplayOrderItems != null;
        }
        public void ExecuteUnloadCommand(string msg)
        {
            DisplayOrderItems = null;
            OriginalOrderItems = null;
            ReloadCounters();
        }
        public void ExecuteUnloadCommand()
        {
            DisplayOrderItems = null;
            OriginalOrderItems = null;
            ReloadCounters();
        }



        //チェックボックスコントロールの全選択・解除コマンド
        private ICommand _checkAllCommand;
        public ICommand CheckAllCommand
        {
            get
            {
                if (_checkAllCommand == null)
                {
                    _checkAllCommand = new RelayCommand<object>(ExecuteCheckAllCommand, CanExecuteCheckAllCommand);
                }
                return _checkAllCommand;
            }
        }
        private bool CanExecuteCheckAllCommand(object arg)
        {
            return DisplayOrderItems != null;
        }
        public void ExecuteCheckAllCommand(object arg)
        {
            if (arg.ToString() == "True")
            {
                foreach (SelectableOrderItem x in collectionView)
                {
                    x.IsChecked = true;
                }
            }
            else
            {
                foreach (SelectableOrderItem x in collectionView.SourceCollection)
                {
                    x.IsChecked = false;
                }
            }
        }


        //SaveChangedコマンド
        private ICommand _importCheckedItemsCommand;
        public ICommand ImportCheckedItemsCommand
        {
            get
            {
                if (_importCheckedItemsCommand == null)
                {
                    _importCheckedItemsCommand = new RelayCommand<string>(ExecuteImportChekedItemsCommand, CanExecuteImportCheckedItemCommand);
                }
                return _importCheckedItemsCommand;
            }
        }
        private bool CanExecuteImportCheckedItemCommand(string arg)
        {
            var a = DisplayOrderItems?.Where(x => x.IsChecked == true)?.Count();
            if (a > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void ExecuteImportChekedItemsCommand(string arg)
        {
            foreach (SelectableOrderItem x in DisplayOrderItems)
            {
                if (x.IsChecked == true)
                {
                    context.OrderItems.Add(x.OrderItem);
                }
            }
            context.SaveChanges();
        }


        #region Dialog

        public ICommand OpenDialogCommand { get; }
        public ICommand AcceptDialogCommand { get; }
        public ICommand CancelDialogCommand { get; }

        private bool _isDialogOpen;
        private object _dialogContent;

        public bool IsDialogOpen
        {
            get { return _isDialogOpen; }
            set
            {
                if (_isDialogOpen == value) return;
                _isDialogOpen = value;
                RaisePropertyChanged();
            }
        }

        public object DialogContent
        {
            get { return _dialogContent; }
            set
            {
                if (_dialogContent == value) return;
                _dialogContent = value;
                RaisePropertyChanged();
            }
        }

        private async void OpenDialog(object obj)
        {
            IsDialogOpen = true;

            ExecuteUnloadCommand();


            DialogContent = new LoadingTimeMessageDialog();
            await Task.Factory.StartNew(() =>
            {
                OriginalOrderItems = orderItem.Load(MakeSQL());
            })
                .ContinueWith((t, _) =>
                {
                    this.DisplayOrderItems = new DispatchObservableCollection<SelectableOrderItem>();

                    foreach (var x in this.OriginalOrderItems)
                    {
                        this.DisplayOrderItems.Add(new SelectableOrderItem { OrderItem = x });
                    }

                    IsDialogOpen = false;

                },
                null,
                TaskScheduler.FromCurrentSynchronizationContext());


            this.ReloadCounters();

            collectionView = CollectionViewSource.GetDefaultView(DisplayOrderItems) as ListCollectionView;

            MyMessageQueue = new SnackbarMessageQueue();
            string msg;

            if (AllCategoryCount == 0) msg = "受注データがありません。";
                msg = "受注データの取得に成功しました。";

            MyMessageQueue.Enqueue(msg, "確認", () => IsSnackbarActive = false);

        }

        private void CancelSample4Dialog(object obj)
        {
            IsDialogOpen = false;
        }

        #endregion


        public SnackbarMessageQueue _myMessageQueue;
        public SnackbarMessageQueue MyMessageQueue
        {
            get { return _myMessageQueue; }
            set
            {
                _myMessageQueue = value;
                RaisePropertyChanged();
            }
        }




        #region Snackbar
        private bool _isSnackbarActive;
        public bool IsSnackbarActive
        {
            get { return _isSnackbarActive; }
            set
            {
                _isSnackbarActive = value;
                RaisePropertyChanged();
            }
        }
        


        #endregion

    }

    #region EntityClass
    
    //VM用エンティティクラス
    public class SelectableOrderItem : INotifyPropertyChanged
    {
        private bool _isChecked = false;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    RaisePropertyChanged();
                }
            }
        }

        private OrderItem _orderItem;
        public OrderItem OrderItem
        {
            get { return _orderItem; }
            set
            {
                if (_orderItem != value)
                {
                    _orderItem = value;
                    RaisePropertyChanged();
                }
            }
        }


        private OrderItem _productCodeItem;
        public OrderItem ProductCodeItem
        {
            get { return _productCodeItem; }
            set
            {
                if (_productCodeItem != value)
                {
                    _productCodeItem = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
    #endregion
}

