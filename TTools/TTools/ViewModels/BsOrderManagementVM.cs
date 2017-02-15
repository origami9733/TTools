using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using TTools.Domain;
using TTools.EF;
using TTools.Models;
using TTools.Views;

namespace TTools.ViewModels
{
    public class BsOrderManagementVM : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region プロパティの変更通知
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region エラー管理
        //発生中のエラーを保持するコレクション
        readonly Dictionary<string, string> _currentErrors = new Dictionary<string, string>();

        //エラーの変更通知
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        private void OnErrorsChanged(string propertyName)
        {
            this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        //エラー保持状態
        public bool HasErrors
        {
            get { return _currentErrors.Count > 0; }
        }

        //エラーの追加・削除
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

        //エラー情報の問い合わせ
        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_currentErrors.ContainsKey(propertyName))
                return null;

            return _currentErrors[propertyName];
        }

        //エラー条件とエラーメッセージの指定
        protected void ValidateProperty(string propertyName, object value)
        {
            switch (propertyName)
            {
                default:
                    break;
            }
        }
        #endregion

        #region コンストラクタ
        public BsOrderManagementVM()
        {
            this.IsDialogOpen = true;
            this.Init();

            //ロード中専用のダイログ画面をセット
            this.DialogContent = new LoadingTimeMessageDialog();
            Load();
        }
        #endregion

        #region 変数
        private TechnoDB context;
        private ICollectionView collectionView;
        #endregion

        #region プロパティ
        /// <summary>
        /// ディスパッチャを実装したコレクションクラス
        /// </summary>
        private DispatchObservableCollection<DisplayOrderManagementItem> _displayOrderManagementItems;
        public DispatchObservableCollection<DisplayOrderManagementItem> DisplayOrderManagementItems
        {
            get { return _displayOrderManagementItems; }
            set
            {
                if (_displayOrderManagementItems == value) return;
                _displayOrderManagementItems = value;
                RaisePropertyChanged();
            }
        }


        //チェックボックスヘッダーの状態を保持するプロパティ
        public bool CheckBoxColumnHeaderIsChecked
        {
            get { return DisplayOrderManagementItems?.Count(x => x.IsChecked == true) > 0; }
        }
        //ダイアログ関連
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
        /// <summary>
        /// 選択中の状態を保持するプロパティ
        /// </summary>
        private string _selectedCategory;
        private Object _selectedRowItem;
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
        public Object SelectedRowItem
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
        #endregion


        #region コマンド
        //チェックボックスヘッダーの操作コマンド
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
        public void ExecuteCheckAllCommand(object arg)
        {
            if (arg.ToString() == "True")
            {
                foreach (DisplayOrderManagementItem a in collectionView) a.IsChecked = true;
            }
            else
            {
                foreach (DisplayOrderManagementItem a in DisplayOrderManagementItems) a.IsChecked = false;
            }
        }
        private bool CanExecuteCheckAllCommand(object arg)
        {
            return DisplayOrderManagementItems != null;
        }
        //変更の保存コマンド
        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand<object>(ExecuteSaveCommand, CanExecuteSaveCommand);
                }
                return _saveCommand;
            }
        }
        public void ExecuteSaveCommand(object arg)
        {

            Save();
        }
        private bool CanExecuteSaveCommand(object arg)
        {
            return DisplayOrderManagementItems != null;
        }
        //初期化してからリフレッシュするコマンド
        private ICommand _refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (_refreshCommand != null) return _refreshCommand;
                _refreshCommand = new RelayCommand<object>(ExecuteRefreshCommand, CanExecuteRefreshCommand);
                return _refreshCommand;
            }
        }
        public void ExecuteRefreshCommand(object arg)
        {
            Init();
            Load();
        }
        private bool CanExecuteRefreshCommand(object arg)
        {
            return DisplayOrderManagementItems != null;
        }

        //初期化してからリフレッシュするコマンド
        private ICommand _groupingCommand;
        public ICommand GroupingCommand
        {
            get
            {
                if (_groupingCommand != null) return _refreshCommand;
                _groupingCommand = new RelayCommand<object>(ExecuteGroupingCommand, CanExecuteGroupingCommandd);
                return _refreshCommand;
            }
        }
        public void ExecuteGroupingCommand(object arg)
        {
        }
        private bool CanExecuteGroupingCommandd(object arg)
        {
            return DisplayOrderManagementItems != null;
        }

        private ICommand _groupHeaderAddCommand;
        public ICommand GroupHeaderAddCommand
        {
            get
            {
                if (_groupHeaderAddCommand != null) return _groupHeaderAddCommand;
                _groupHeaderAddCommand = new RelayCommand<string>(ExecuteGroupHeaderAddCommand);
                return _groupHeaderAddCommand;
            }
        }
        public void ExecuteGroupHeaderAddCommand(string arg)
        {
            EItemSelect(arg);
        }
        #endregion



        #region メソッド
        /// <summary>
        /// 初期化を行う。
        /// </summary>
        private void Init()
        {
            context = null;
            collectionView = null;
            DisplayOrderManagementItems = null;
        }

        /// <summary>
        /// 保存を行う。
        /// </summary>
        private void Save()
        {
            foreach (var a in context.ProductItems.Local)
            {
                if (!context.ProductItems.ToArray().Contains(a))
                {
                    if (!string.IsNullOrEmpty(a.LongName)) context.ProductItems.Add(a);
                }
            }

            context.SaveChanges();
            Init();
            Load();
        }


        private void EItemSelect(string ProductID)
        {
            var DialogVM = new EItemSelectDialogVM();
            var DialogView = new EItemSelectDialog() { DataContext = DialogVM };
            DialogContent = DialogView;

            DialogView.DG1.PreviewMouseDoubleClick += (x, _) =>
            {
                if(DialogVM.SelectedEitem == null)
                {
                    return;
                }
                else
                {
                    YorNConfirmWindow ConfirmView = new YorNConfirmWindow();
                    ConfirmView.Message.Text = 
                        DialogVM.SelectedEitem.ID +" を " + ProductID + " に加えますか？";

                    ConfirmView.AcceptBT.Click += (a, b) => { AddProductChildItem(ProductID, DialogVM.SelectedEitem.ID); };
                    ConfirmView.CancelBT.Click += (a, b) => { DialogContent = DialogView; };
                    DialogContent = ConfirmView;
                }
            };

            DialogView.AddBT.Click += (x, _) =>
            {
                if (DialogVM.SelectedEitem == null)
                {
                    return;
                }
                else
                {
                    YorNConfirmWindow ConfirmView = new YorNConfirmWindow();
                    ConfirmView.Message.Text =
                        DialogVM.SelectedEitem.ID + "を" + ProductID + "に加えますか？";

                    ConfirmView.AcceptBT.Click += (a, b) => { AddProductChildItem(ProductID, DialogVM.SelectedEitem.ID); };
                    ConfirmView.CancelBT.Click += (a, b) => { DialogContent = DialogView; };

                    DialogContent = ConfirmView;
                }
            };

            DialogView.CancelBT.Click += (x, _) => { IsDialogOpen = false; };

            IsDialogOpen = true;
        }


        private void AddProductChildItem(string productID,string eItemId)
        {
            bool relationContain = context.Relationships.ToList().Where(x => x.ProductId == productID & x.EItemId == eItemId).Count() > 0;

            if (relationContain == true)
            {
                var AcceptDialogView = new AcceptConfirmDialog();
                AcceptDialogView.Message.Text = "選択されたアイテムは既に構成情報に含まれています。";
            }
            else
            {
                if(DisplayOrderManagementItems.Where(x => x.ProductItem.ProductId == productID & x.RelationItem == null).Count() > 0)
                {
                    var addItem = new Relationship() { ProductId = productID, EItemId = eItemId };
                    foreach(var a in DisplayOrderManagementItems)
                    {
                        if(a.ProductItem.ProductId == productID& a.RelationItem == null)
                        {
                            a.RelationItem = addItem;
                        }
                    }

                    context.Relationships.Add(new Relationship() { ProductId = productID, EItemId = eItemId });
                }
                else
                {
                    var rItem = new Relationship() { ProductId = productID, EItemId = eItemId };
                    var dummyList = DisplayOrderManagementItems.Where(x => x.ProductItem.ProductId == productID).Select(x => x.OrderItem.伝票ＮＯ).ToList();
                    List<string> OrderNoList = new List<string>();

                    foreach(var a in dummyList)
                    {
                        if(!OrderNoList.Contains(a))
                        {
                            OrderNoList.Add(a);
                        }
                    }

                    foreach(var a in OrderNoList)
                    {
                        var oItem = context.OrderItems.Where(x => x.伝票ＮＯ == a).First();
                        var pItem = context.ProductItems.Where(x => x.ProductId == productID).First();
                        var eItem = context.EItems.Where(x => x.ID == eItemId).First();

                        var addItem = new DisplayOrderManagementItem()
                        {
                            OrderItem = oItem,
                            ProductItem = pItem,
                            RelationItem = rItem,
                            EItem = eItem,
                        };

                        DisplayOrderManagementItems.Add(addItem);
                    }
                    context.Relationships.Add(rItem);
                }
            }

            IsDialogOpen = false;
        }





        private void Grouping()
        {
            collectionView.GroupDescriptions.Clear();
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("OrderItem.商品コード"));
            collectionView.Refresh();
        }

        private async void Load()
        {
            context = new TechnoDB();
            context.OrderItems.ToList();
            context.ProductItems.ToList();
            context.EItems.ToList();
            context.Relationships.ToList();

            DisplayOrderManagementItems = new DispatchObservableCollection<DisplayOrderManagementItem>();

            await Task.Factory.StartNew(() =>
            {
                foreach (var oItem in context.OrderItems.Local)
                {
                    var pItem = context.ProductItems.Local.Where(x => x.ProductId == oItem.商品コード).First();
                    var children = context.Relationships.Local.Where(x => x.ProductId == oItem.商品コード);

                    if (children.Count() == 0)
                    {
                        var AddItem = new DisplayOrderManagementItem();

                        AddItem.IsChecked = false;
                        AddItem.OrderItem = oItem;
                        AddItem.ProductItem = pItem;

                        DisplayOrderManagementItems.Add(AddItem);
                    }
                    else
                    {
                        for (int i = 0; i < children.Count(); i++)
                        {
                            var AddItem = new DisplayOrderManagementItem();

                            AddItem.IsChecked = false;
                            AddItem.OrderItem = oItem;
                            AddItem.ProductItem = pItem;
                            AddItem.RelationItem = children.ToList()[i];
                            AddItem.EItem = context.EItems.Local.Where(x => x.ID == AddItem.RelationItem.EItemId).First();

                            DisplayOrderManagementItems.Add(AddItem);
                        }
                    }
                }
            }).ContinueWith((Action<Task, object>)((t, _) =>
            {
                foreach (var x in DisplayOrderManagementItems)
                {
                    Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                        h => x.PropertyChanged += h,
                        h => x.PropertyChanged -= h)
                        .Subscribe((System.Reactive.EventPattern<PropertyChangedEventArgs> e) =>
                        {
                            RaisePropertyChanged("CheckBoxColumnHeaderIsChecked");
                        });
                }
                IsDialogOpen = false;
                this.collectionView = CollectionViewSource.GetDefaultView(this.DisplayOrderManagementItems);
            }), null, TaskScheduler.FromCurrentSynchronizationContext());

            collectionView.GroupDescriptions.Clear();
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("OrderItem.伝票ＮＯ"));
        }



        #endregion

        #region DataGridColumn表示設定
        private bool _isVisible商品コード;
        public bool IsVisible商品コード
        {
            get { return _isVisible商品コード; }
            set
            {
                if (_isVisible商品コード == value) return;
                _isVisible商品コード = value;
                RaisePropertyChanged();
            }
        }
        #endregion
    }

    //VM用エンティティクラス
    public class DisplayOrderManagementItem : INotifyPropertyChanged
    {
        #region 変数
        private bool _isChecked;
        private OrderItem _orderItem;
        private ProductItem _productItem;
        private Relationship _relationItem;
        private EItem _eItem;
        #endregion

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked == value) return;
                _isChecked = value;
                RaisePropertyChanged();
            }
        }
        public OrderItem OrderItem
        {
            get { return _orderItem; }
            set
            {
                if (_orderItem == value) return;
                _orderItem = value;
                RaisePropertyChanged();
            }
        }
        public ProductItem ProductItem
        {
            get { return _productItem; }
            set
            {
                if (_productItem == value) return;
                _productItem = value;
                RaisePropertyChanged();
            }
        }
        public Relationship RelationItem
        {
            get { return _relationItem; }
            set
            {
                if (_relationItem == value) return;
                _relationItem = value;
                RaisePropertyChanged();
            }
        }
        public EItem EItem
        {
            get { return _eItem; }
            set
            {
                if (_eItem == value) return;
                _eItem = value;
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