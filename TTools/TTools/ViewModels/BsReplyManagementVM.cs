using MaterialDesignThemes.Wpf;
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
    public class BsReplyManagementVM : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region プロパティ変更通知インターフェース
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        #region エラー変更通知インターフェース
        //発生中のエラー保持
        readonly Dictionary<string, string> _currentErrors = new Dictionary<string, string>();
        //エラーの追加
        protected void AddError(string propertyName, string error)
        {
            if (!_currentErrors.ContainsKey(propertyName))
                _currentErrors[propertyName] = error;

            OnErrorsChanged(propertyName);
        }
        //エラーの取り下げ
        protected void RemoveError(string propertyName, string error)
        {
            if (_currentErrors.ContainsKey(propertyName))
                _currentErrors.Remove(propertyName);

            OnErrorsChanged(propertyName);
        }
        //エラーの変更通知
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        private void OnErrorsChanged(string propertyName)
        {
            this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
        //エラー情報の問い合わせ
        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_currentErrors.ContainsKey(propertyName))
                return null;

            return _currentErrors[propertyName];
        }
        //エラー保持状態
        public bool HasErrors
        {
            get { return _currentErrors.Count > 0; }
        }
        #endregion
        #region スナックバー関連
        private bool _isSnackbarActive;
        public bool IsSnackbarActive
        {
            get { return _isSnackbarActive; }
            set
            {
                if (_isSnackbarActive == value) return;
                _isSnackbarActive = value;
                RaisePropertyChanged();
            }
        }
        private SnackbarMessageQueue _messageQueue;
        public SnackbarMessageQueue MessageQueue
        {
            get { return _messageQueue; }
            set
            {
                if (_messageQueue == value) return;
                _messageQueue = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// スナックバーを表示する
        /// </summary>
        /// <param name="msg"></param>
        private void ShowSnackbar(string msg)
        {
            MessageQueue = new SnackbarMessageQueue();
            string snackbarMessage = msg;
            this.MessageQueue.Enqueue(snackbarMessage, "確認", () => IsSnackbarActive = false);
        }
        #endregion
        #region ダイアログ関連
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
        #endregion
        #region エラー詳細管理
        protected void ValidateProperty(string propertyName, object value)
        {
            switch (propertyName)
            {
                default:
                    break;
            }
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
        private string _orderFilterStr;
        public string OrderFilterStr
        {
            get { return _orderFilterStr; }
            set
            {
                if (_orderFilterStr == value) return;
                _orderFilterStr = value;
                SetFilter();
            }
        }
        private string _productFilterStr;
        public string ProductFilterStr
        {
            get { return _productFilterStr; }
            set
            {
                if (_productFilterStr == value) return;
                _productFilterStr = value;
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



        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="arg"></param>
        public BsReplyManagementVM(MainWindowVM arg)
        {
            masterVM = arg;
        }
        /// <summary>
        /// フィルター状態を設定する
        /// </summary>
        private void SetFilter()
        {
            if (OrderFilterStr == null & ProductFilterStr == null)
            {
                collectionView.Filter = null;
                return;
            }
            else
            {
                collectionView.Filter = new Predicate<object>(x =>
                {
                    var item = (DisplayReplyManagementItem)x;
                    if (string.IsNullOrEmpty(OrderFilterStr) == false)
                    {
                        if (item.OrderItem.伝票ＮＯ.Contains(OrderFilterStr) == true) return true;
                    }
                    return false;
                });
            }
        }
        /// <summary>
        /// ロード
        /// </summary>
        private async void Load(string replyStatus)
        {
            UnReadObservables();

            DialogContent = new LoadingProgressDialog();
            vendorItems = TpicsDbContext.LoadVendor();

            context = new TechnoDB();
            collectionView = null;
            ReplyItems = null;
            ReplyItems = new DispatchObservableCollection<DisplayReplyManagementItem>();
            BindingOperations.EnableCollectionSynchronization(ReplyItems, new object());
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
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("OrderItem.伝票ＮＯ"));


            IsDialogOpen = false;
        }
        /// <summary>
        /// 納期返答のアップデートSQLを作成
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="newValue"></param>
        /// <param name="targetOrderNo"></param>
        /// <returns></returns>
        private string GenerateUpdateReplyDateSql(string newValue, string targetOrderNo)
        {
            string sqlStr = "";
            sqlStr = $"UPDATE dbo.GS_0000101 SET 発注取引先出荷予定年月日 = '{newValue}' 伝送フラグ = '1' WHERE 伝票ＮＯ = '{targetOrderNo}'";
            return sqlStr;
        }
        /// <summary>
        /// 運送情報のアップデートSQLを作成
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GenerateUpdateShippingDataSql(OrderItem item)
        {
            string sqlStr = "";
            sqlStr = $"UPDATE dbo.GS_0000101 SET 送り状ＮＯ = '{item.InvoiceNo}', 運送会社名称 = '{item.ShippingCompanyName}', 運送会社連絡先電話番号 = '{item.ShippingCompanyTel}', 伝送フラグ = '1' WHERE 伝票ＮＯ = '{item.伝票ＮＯ}'";
            return sqlStr;
        }
        /// <summary>
        /// 監視状態を設定
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
            foreach (var a in ReplyItems)
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

            switch (e.EventArgs.PropertyName)
            {
                case nameof(OrderItem.InvoiceNo):
                    context.SaveChanges();
                    break;

                case nameof(OrderItem.ShippingCompanyName):
                    context.SaveChanges();
                    break;

                case nameof(OrderItem.ShippingCompanyTel):
                    context.SaveChanges();
                    break;
            }
        }
        /// <summary>
        /// 監視プロパティに変更があった時に実行される
        /// </summary>
        /// <param name="e"></param>
        private void ChangeProductItemProperty(EventPattern<PropertyChangedEventArgs> e)
        {
            switch (e.EventArgs.PropertyName)
            {
                case nameof(ProductItem.LeadTime):
                    context.SaveChanges();
                    var item = ReplyItems.Where(x => x.ProductItem == e.Sender).First();
                    item.ReplyDate = Calendar.LeadTimeCalc(item.ProductItem.LeadTime);
                    
                    break;
            }
        }
        /// <summary>
        /// 運送情報を更新して表示用コレクションから削除する
        /// </summary>
        /// <param name="oItem"></param>
        /// <param name="items"></param>
        private void UpdateShippingData(OrderItem oItem, ObservableCollection<DisplayReplyManagementItem> items)
        {
            try
            {
                gsContext.ExecuteSQL(GenerateUpdateShippingDataSql(items[0].OrderItem));
            }
            catch
            {
                ShowSnackbar("更新は失敗しました。");
            }
            finally
            {
                items[0].OrderItem.ReplyStatus = "1";
                context.SaveChanges();

                foreach (var a in items)
                {
                    ReplyItems.Remove(a);
                }
                context.SaveChanges();

                ShowSnackbar("更新は成功しました。");
            }
        }
        /// <summary>
        /// 返答ステータスを変更する。
        /// </summary>
        /// <param name="items"></param>
        private void ChangeReplyStatus(ObservableCollection<DisplayReplyManagementItem> items)
        {
            items[0].OrderItem.ReplyStatus = "1";
            context.SaveChanges();

            foreach (var a in items)
            {
                ReplyItems.Remove(a);
            }
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
            Load(null);
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
            items[0].OrderItem.ReplyDate = items[0].ReplyDate;
            string sql = GenerateUpdateReplyDateSql(items[0].OrderItem.ReplyDate, items[0].OrderItem.伝票ＮＯ);
            gsContext.ExecuteSQL(sql);
            context.SaveChanges();
        }

        private ICommand _replyShippingDataCommand;
        public ICommand ReplyShippingDataCommand
        {
            get
            {
                if (_replyShippingDataCommand != null) return _replyShippingDataCommand;
                _replyShippingDataCommand = new RelayCommand<object>(ExecuteReplyShippingDataCommand, CanExecuteReplyShippingDataCommand);
                return _replyShippingDataCommand;
            }
        }
        public void ExecuteReplyShippingDataCommand(object arg)
        {
            var args = (ReadOnlyObservableCollection<object>)arg;
            ObservableCollection<DisplayReplyManagementItem> items = new ObservableCollection<DisplayReplyManagementItem>();

            foreach (var a in args)
            {
                items.Add((DisplayReplyManagementItem)a);
            }
            var oItem = items[0].OrderItem;
            var dialog = new YesNoConfirmDialog();
            dialog.Message.Text = $"下記の内容で回答を実行しますか？\r\n\r\n" +
                                $"運送会社名：{oItem.ShippingCompanyName}\r\n" +
                                 $"運送会社TEL：{oItem.ShippingCompanyTel}\r\n" +
                                  $"送り状NO：{oItem.InvoiceNo}";
            dialog.AcceptBT.Click += (x, y) => { IsDialogOpen = false; UpdateShippingData(oItem, items); };
            dialog.CancelBT.Click += (x, y) => { IsDialogOpen = false; return; };

            DialogContent = dialog;
            IsDialogOpen = true;
        }
        public bool CanExecuteReplyShippingDataCommand(object arg)
        {
            var items = (ReadOnlyObservableCollection<object>)arg;

            if (items.Count() == 0) return false;
            var item = (DisplayReplyManagementItem)items[0] ?? null;

            if (item == null) return false;
            if (item.OrderItem.ReplyDate == null) return false;

            var oItem = item.OrderItem;
            if (string.IsNullOrEmpty(oItem.InvoiceNo) == false || string.IsNullOrEmpty(oItem.ShippingCompanyName) == false || string.IsNullOrEmpty(oItem.ShippingCompanyTel) == false)
            {
                return true;
            }
            else { return false; }
        }

        private ICommand _changeReplyStatusCommand;
        public ICommand ChangeReplyStatusCommand
        {
            get
            {
                if (_changeReplyStatusCommand != null) return _changeReplyStatusCommand;
                _changeReplyStatusCommand = new RelayCommand<object>(ExecuteCangeReplyStatusCommand);
                return _changeReplyStatusCommand;
            }
        }
        public void ExecuteCangeReplyStatusCommand(object arg)
        {
            var args = (IList<object>)arg;
            ObservableCollection<DisplayReplyManagementItem> items = new ObservableCollection<DisplayReplyManagementItem>();

            foreach (var a in args)
            {
                items.Add((DisplayReplyManagementItem)a);
            }

            if (items[0].OrderItem.ReplyDate == null)
            {
                var dialog = new YesNoConfirmDialog();
                dialog.Message.Text = "納期回答がされていませんが非表示化しますか？";
                dialog.AcceptBT.Click += (x, y) => { ChangeReplyStatus(items); IsDialogOpen = false; };
                dialog.CancelBT.Click += (x, y) => { IsDialogOpen = false; };
                DialogContent = dialog;
                IsDialogOpen = true;
            }
            else
            {
                ChangeReplyStatus(items);
            }
        }

    }
}
