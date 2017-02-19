using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using TTools.Domain;
using TTools.EF;
using TTools.Models;
using TTools.Views;

namespace TTools.ViewModels
{
    public class OrderEditVM : INotifyPropertyChanged
    {
        #region プロパティ変更通知
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region ローカル変数
        private MainWindowVM masterVM;
        private TechnoDB context;
        private List<IDisposable> ObservableList = new List<IDisposable>();
        private ICollectionView collectionView;
        #endregion

        #region プロパティ
        private bool _isDialogOpen;
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

        private object _dialogContent;
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

        private object _selectedRowitem;
        public object SelectedRowItem
        {
            get { return _selectedRowitem; }
            set
            {
                if (_selectedRowitem == value) return;
                _selectedRowitem = value;
                RaisePropertyChanged();
            }
        }

        private DispatchObservableCollection<OrderItem> _orderItems;
        public DispatchObservableCollection<OrderItem> OrderItems
        {
            get { return _orderItems; }
            set
            {
                if (_orderItems == value) return;
                _orderItems = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="argVM"></param>
        public OrderEditVM(MainWindowVM argVM)
        {
            masterVM = argVM;
        }


        private ICommand _loadCommand;
        public ICommand LoadCommand
        {
            get
            {
                if (_loadCommand != null) return _loadCommand;
                _loadCommand = new RelayCommand<object>(ExecuteLoadCommand);
                return _loadCommand;
            }
        }
        public void ExecuteLoadCommand(object arg)
        {
            DialogContent = new LoadingTimeMessageDialog();
            IsDialogOpen = true;
            Load();
        }
        private BeautyGsContext gsContext = new BeautyGsContext();


        private async void Load()
        {
            context = new TechnoDB();

            await Task.Run(() =>
            {
                context.OrderItems.ToList();
                OrderItems = new DispatchObservableCollection<OrderItem>(context.OrderItems.Local);
            });

            SetObservable();
            collectionView = CollectionViewSource.GetDefaultView(OrderItems);
            IsDialogOpen = false;
        }

        private string MakeUpdateSqlString(string propertyName, string newValue, string targetOrderNo)
        {
            string sqlStr = "";

            switch (propertyName)
            {
                case nameof(OrderItem.InvoiceNo):
                    sqlStr = "UPDATE dbo.GS_0000101 SET 送り状ＮＯ = '" + newValue + "' WHERE 伝票ＮＯ = '" + targetOrderNo + "'";
                    break;
                case nameof(OrderItem.ShippingCompanyName):
                    sqlStr = "UPDATE dbo.GS_0000101 SET 運送会社名称 = '" + newValue + "' WHERE 伝票ＮＯ = '" + targetOrderNo + "'";
                    break;
                case nameof(OrderItem.ShippingCompanyTel):
                    sqlStr = "UPDATE dbo.GS_0000101 SET 運送会社連絡先電話番号 = '" + newValue + "' WHERE 伝票ＮＯ = '" + targetOrderNo + "'";
                    break;
            }

            return sqlStr;
        }

        /// <summary>
        /// 監視状態を設定する
        /// </summary>
        private void SetObservable()
        {
            //監視状態を設定 OrderItem
            foreach (var x in OrderItems)
            {
                var a = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => x.PropertyChanged += h,
                    h => x.PropertyChanged -= h)
                    .Subscribe((System.Reactive.EventPattern<PropertyChangedEventArgs> e) =>
                    {
                        ChangeTargetProperty(e);
                    });
                ObservableList.Add(a);
            }
        }

        /// <summary>
        /// 監視プロパティに変更があった時に実行される
        /// </summary>
        /// <param name="e"></param>
        private void ChangeTargetProperty(EventPattern<PropertyChangedEventArgs> e)
        {
            switch (e.EventArgs.PropertyName)
            {
                case nameof(OrderItem.InvoiceNo):
                    var a = (OrderItem)SelectedRowItem;
                    var str1 = MakeUpdateSqlString(nameof(OrderItem.InvoiceNo), a.InvoiceNo, a.伝票ＮＯ);

                    gsContext.Write(str1);
                    context.SaveChanges();
                    break;

                case nameof(OrderItem.ShippingCompanyName):
                    var b = (OrderItem)SelectedRowItem;
                    var str2 = MakeUpdateSqlString(nameof(OrderItem.ShippingCompanyName), b.ShippingCompanyName, b.伝票ＮＯ);

                    gsContext.Write(str2);
                    context.SaveChanges();
                    break;

                case nameof(OrderItem.ShippingCompanyTel):
                    var c = (OrderItem)SelectedRowItem;
                    var str3 = MakeUpdateSqlString(nameof(OrderItem.ShippingCompanyTel), c.ShippingCompanyTel, c.伝票ＮＯ);

                    gsContext.Write(str3);
                    context.SaveChanges();
                    break;
            }
        }

        /// <summary>
        /// 監視状態を解除する
        /// </summary>
        private void UnReadObservables()
        {
            foreach (var a in ObservableList)
            {
                a.Dispose();
            }
        }
    }
}
