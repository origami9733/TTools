using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using TTools.Domain;
using TTools.EF;
using TTools.Models;
using TTools.Views;

namespace TTools.ViewModels
{
    public class BsReplyManagementVM : INotifyPropertyChanged
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
        private List<IDisposable> observableList = new List<IDisposable>();
        private ICollectionView collectionView;
        private BeautyGsContext gsContext = new BeautyGsContext();
        private DispatchObservableCollection<VendorItem> vendorItems;
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

        private DispatchObservableCollection<DisplayReplyManagementItem> _replyItems;
        public DispatchObservableCollection<DisplayReplyManagementItem> ReplyItems
        {
            get { return _replyItems; }
            set
            {
                if (_replyItems == value) return;
                _replyItems = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="arg"></param>
        public BsReplyManagementVM(MainWindowVM arg)
        {
            masterVM = arg;
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
            DialogContent = new LoadingProgressDialog();
            IsDialogOpen = true;
            Load();
        }

        private ICommand _replyLeadTimeCommand;
        public ICommand ReplyLeadTimeCommand
        {
            get
            {
                if (_replyLeadTimeCommand != null) return _replyLeadTimeCommand;
                _replyLeadTimeCommand = new RelayCommand<object>(ExecuteReplyLeadTimeCommand);
                return _replyLeadTimeCommand;
            }
        }
        public void ExecuteReplyLeadTimeCommand(object arg)
        {
            var args = (ReadOnlyObservableCollection<object>)arg;
            ObservableCollection<DisplayReplyManagementItem> items = new ObservableCollection<DisplayReplyManagementItem>();

            foreach (var a in args)
            {
                items.Add((DisplayReplyManagementItem)a);
            }

            string returnDate = Calendar.LeadTimeCalc(items[0].ProductItem.LeadTime);

            foreach (var a in items)
            {
                a.ReplyDate = returnDate;
            }
        }


        /// <summary>
        /// データをロードする。
        /// </summary>
        private async void Load()
        {
            DialogContent = new LoadingProgressDialog();
            vendorItems = TpicsDbContext.LoadVendor();

            context = new TechnoDB();
            ReplyItems = new DispatchObservableCollection<DisplayReplyManagementItem>();

            IsDialogOpen = true;

            await Task.Run(() =>
            {
                context.OrderItems.ToList();
                context.RelationItems.ToList();
                context.ProductItems.ToList();
                context.EItems.ToList();

                //BSカテゴリーかつ返答ステータスがNullのみを取得
                foreach (var oItem in context.OrderItems.Where(x => x.フラグ１ == "0" & x.ReplyStatus == null))
                {
                    var pItem = context.ProductItems.Local.Where(x => x.ProductId == oItem.商品コード).First();
                    var children = context.RelationItems.Local.Where(x => x.ProductId == oItem.商品コード);

                    //リレーションが存在しない場合、rItemとeItemはNULL
                    if (children.Count() == 0)
                    {
                        var AddItem = new DisplayReplyManagementItem();
                        AddItem.OrderItem = oItem;
                        AddItem.ProductItem = pItem;

                        ReplyItems.Add(AddItem);
                    }
                    else //リレーションが存在する場合
                    {
                        for (int i = 0; i < children.Count(); i++)
                        {
                            var AddItem = new DisplayReplyManagementItem();
                            AddItem.OrderItem = oItem;
                            AddItem.ProductItem = pItem;
                            AddItem.RelationItem = children.ToList()[i];
                            AddItem.EItem = context.EItems.Where(x => x.Id == AddItem.RelationItem.EItemId).First();
                            if (AddItem.EItem.VendorId != null)
                            {
                                AddItem.VendorItem = vendorItems.Where(x => x.Id == AddItem.EItem.VendorId).First();
                            }

                            ReplyItems.Add(AddItem);
                        }
                    }
                }
            });

            SetVirtualReplyDate();
            SetObservable();
            collectionView = CollectionViewSource.GetDefaultView(ReplyItems);
            PropertyGroupDescription aa = new PropertyGroupDescription("OrderItem.伝票ＮＯ");

            var d = new DrivedObject();
            await Task.Run(async () =>
            {
                if (!d.CheckAccess())
                {
                    await d.Dispatcher.InvokeAsync(() => collectionView.GroupDescriptions.Add(aa));
                }

            });

            IsDialogOpen = false;
        }

        /// <summary>
        /// GSサーバーに対するアップデートSQL文字列を作成する
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="newValue"></param>
        /// <param name="targetOrderNo"></param>
        /// <returns></returns>
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
            foreach (var x in ReplyItems.Select(x => x.OrderItem))
            {
                var a = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => x.PropertyChanged += h,
                    h => x.PropertyChanged -= h)
                    .Subscribe((System.Reactive.EventPattern<PropertyChangedEventArgs> e) =>
                    {
                        ChangeOrderItemProperty(e);
                    });
                observableList.Add(a);
            }
            //監視状態を設定 ProductItem
            foreach (var x in ReplyItems.Select(x => x.ProductItem))
            {
                var a = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => x.PropertyChanged += h,
                    h => x.PropertyChanged -= h)
                    .Subscribe((System.Reactive.EventPattern<PropertyChangedEventArgs> e) =>
                    {
                        ChangeProductItemProperty(e);
                    });
                observableList.Add(a);
            }
        }
        /// <summary>
        /// 監視状態を解除する
        /// </summary>
        private void UnReadObservables()
        {
            foreach (var a in observableList)
            {
                a.Dispose();
            }
        }
        /// <summary>
        /// 返答日時をセットする
        /// </summary>
        private void SetVirtualReplyDate()
        {
            foreach(var a in ReplyItems)
            {
                a.ReplyDate = Calendar.LeadTimeCalc(a.ProductItem.LeadTime);
            }
        }
        /// <summary>
        /// 監視プロパティに変更があった時に実行される
        /// </summary>
        /// <param name="e"></param>
        private void ChangeOrderItemProperty(EventPattern<PropertyChangedEventArgs> e)
        {
            var item = ReplyItems.Where(x => x.OrderItem == e.Sender).First();
            string sql;

            switch (e.EventArgs.PropertyName)
            {
                case nameof(OrderItem.InvoiceNo):
                    sql = MakeUpdateSqlString(nameof(OrderItem.InvoiceNo), item.OrderItem.InvoiceNo, item.OrderItem.伝票ＮＯ);

                    gsContext.ExecuteSQL(sql);
                    context.SaveChanges();
                    break;

                case nameof(OrderItem.ShippingCompanyName):
                    sql = MakeUpdateSqlString(nameof(OrderItem.ShippingCompanyName), item.OrderItem.ShippingCompanyName, item.OrderItem.伝票ＮＯ);

                    gsContext.ExecuteSQL(sql);
                    context.SaveChanges();
                    break;

                case nameof(OrderItem.ShippingCompanyTel):
                    sql = MakeUpdateSqlString(nameof(OrderItem.ShippingCompanyTel), item.OrderItem.ShippingCompanyTel, item.OrderItem.伝票ＮＯ);

                    gsContext.ExecuteSQL(sql);
                    context.SaveChanges();
                    break;
            }
        }
        private void ChangeProductItemProperty(EventPattern<PropertyChangedEventArgs> e)
        {
            switch (e.EventArgs.PropertyName)
            {
                case nameof(ProductItem.LeadTime):
                    context.SaveChanges();
                    var items = ReplyItems.Where(x => x.ProductItem == e.Sender);
                    foreach(var a in items)
                    {
                        a.ReplyDate = Calendar.LeadTimeCalc(a.ProductItem.LeadTime);
                    }
                    break;
            }
        }
    }
}
