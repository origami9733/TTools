using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;
using TTools.Models;
using TTools.EF;
using TTools.Views;
using System.Diagnostics;
using System.Collections.Generic;

namespace TTools.ViewModels
{
    public class ImportOrdersVM : INotifyPropertyChanged, INotifyDataErrorInfo
    {
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
        private ObservableCollection<OrderItem> _originalOrderItems;
        public ObservableCollection<OrderItem> OriginalOrderItems
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
        private ObservableCollection<SelectableOrderItem> _displayOrderItems;
        public ObservableCollection<SelectableOrderItem> DisplayOrderItems
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
        private int? _machineCategoryCount;
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
        private int? _bsCategoryCount;
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
        private int? _plCategoryCount;
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


        //開発用
        private ICommand _testCommand;
        public ICommand TestCommand
        {
            get
            {
                if (_testCommand == null)
                {
                    _testCommand = new RelayCommand<string>(ExecuteTestCommand);
                }
                return _testCommand;
            }
        }
        private void ExecuteTestCommand(string arg)
        {
            CheckBoxColumnIsChecked = false;
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
            if(_currentErrors.ContainsKey(nameof(UpdateDate))==true)
                return false;

            return true;
        }
        private void ExecuteReloadCommand(String arg)
        {
            //アンロード
            ExecuteUnloadCommand();

            var busyMessageDialog = new LoadingTimeMessageDialog
            {
                Message = { Text = "Loading..." }
            };
            DialogHost.Show(busyMessageDialog, "RootDialog");

            //オリジナル受注コレクションの作成
            OriginalOrderItems = orderItem.Load(MakeSQL());
            //インポート画面用コレクションの作成
            DisplayOrderItems = new ObservableCollection<SelectableOrderItem>();
            foreach (var x in OriginalOrderItems)
            {
                DisplayOrderItems.Add(new SelectableOrderItem { OrderItem = x });
            }

            //カウンターリロード
            this.ReloadCounters();

            //フィルター用ビューの取得
            collectionView = CollectionViewSource.GetDefaultView(DisplayOrderItems) as ListCollectionView;
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
    }

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

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

