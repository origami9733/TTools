using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TTools.Domain;
using TTools.EF;
using TTools.Models;

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
            Load();
        }
        #endregion

        #region 変数
        private TechnoDB context;
        private ICollectionView collectionView;
        #endregion

        #region プロパティ
        private DispatcheObservableCollection<DisplayOrderManagementItem> _displayOrderManagementItems;
        private DispatcheObservableCollection<OrderItem> _orderItems;
        private DispatcheObservableCollection<ProductItem> _productItems;

        public DispatcheObservableCollection<DisplayOrderManagementItem> DisplayOrderManagementItems
        {
            get { return _displayOrderManagementItems; }
            set
            {
                if (_displayOrderManagementItems == value) return;
                _displayOrderManagementItems = value;
                RaisePropertyChanged();
            }
        }
        public DispatcheObservableCollection<OrderItem> OrderItems
        {
            get { return _orderItems; }
            set
            {
                if (_orderItems == value) return;
                _orderItems = value;
                RaisePropertyChanged();
            }
        }
        public DispatcheObservableCollection<ProductItem> ProductItems
        {
            get { return _productItems; }
            set
            {
                if (_productItems == value) return;
                _productItems = value;
                RaisePropertyChanged();
            }
        }

        //チェックボックスヘッダーの状態を保持するプロパティ
        public bool CheckBoxColumnHeaderIsChecked
        {
            get { return DisplayOrderManagementItems?.Count(x => x.IsChecked == true) > 0; }
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
            MessageBox.Show("a");
        }
        private bool CanExecuteSaveCommand(object arg)
        {
            return DisplayOrderManagementItems != null;
        }
        private ICommand _refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new RelayCommand<object>(ExecuteRefreshCommand, CanExecuteRefreshCommand);
                }
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

        #endregion



        
        private void Init()
        {
            context = null;
            collectionView = null;

            DisplayOrderManagementItems = null;
            OrderItems = null;
            ProductItems = null;
        }

        private void Save()
        {
            foreach(var a in DisplayOrderManagementItems)
            {
                if (!context.ProductItems.ToArray().Contains(a.ProductItem))
                {
                    if(a.ProductItem.LongName != null) ProductItems.Add(a.ProductItem);

                    if (a.ProductItem.LongName != null) context.ProductItems.Add(a.ProductItem);
                }
            }
            context.SaveChanges();
        }


        private void Load()
        {
            context = new TechnoDB();
            context.OrderItems.ToList();
            context.ProductItems.ToList();

            //派生クラスDispatcheObservableCollectionのコンストラクタを通じて基底クラスへ格納する
            OrderItems = new DispatcheObservableCollection<OrderItem>(context.OrderItems.Local);
            ProductItems = new DispatcheObservableCollection<ProductItem>(context.ProductItems.Local);

            DisplayOrderManagementItems = new DispatcheObservableCollection<DisplayOrderManagementItem>();

            foreach (var orderItem in OrderItems)
            {
                var AddItem = new DisplayOrderManagementItem();

                AddItem.IsChecked = false;
                AddItem.OrderItem = orderItem;
                ProductItem pItem;

                if(ProductItems.Count(y => y.ProductId == orderItem.商品コード) > 0)
                {
                    pItem = ProductItems.Where(y => y.ProductId == orderItem.商品コード).First();
                    AddItem.ProductItem = pItem;
                }
                else
                {
                    pItem = new ProductItem();
                    pItem.ProductId = orderItem.商品コード;
                    AddItem.ProductItem = pItem;
                    ProductItems.Add(pItem);
                }

                DisplayOrderManagementItems.Add(AddItem);
            }

            this.collectionView = CollectionViewSource.GetDefaultView(this.DisplayOrderManagementItems) as ListCollectionView;
        }
    }

    //VM用エンティティクラス
    public class DisplayOrderManagementItem : INotifyPropertyChanged
    {
        private bool _isChecked;
        private OrderItem _orderItem;
        private ProductItem _productItem;

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

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
