using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
        #region NotifyPropertyインターフェース
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region NotifyDataErrorインターフェース

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

        public BsOrderManagementVM(MainWindowVM arg)
        {
            masterVM = arg;
        }

        private MainWindowVM masterVM;
        private TechnoDB context;
        private ICollectionView collectionView;
        private List<IDisposable> ObservableList = new List<IDisposable>();

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


        public bool ForObservableChangesStatus { get; set; } = true;

        #region ダイアログ関連のプロパティ
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
        #endregion

        #region Selectedプロパティ
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

        #region DataGridColumn表示設定プロパティ
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

        private ICommand _loadCommand;
        public ICommand LoadCommand
        {
            get
            {
                if (_loadCommand != null) return _loadCommand;
                _loadCommand = new RelayCommand<string>(ExecuteLoadCommand);
                return _loadCommand;
            }
        }
        public async void ExecuteLoadCommand(string arg)
        {
            await Load();

            IsDialogOpen = false;
        }

        private ICommand _orderRegistrationCommand;
        public ICommand OrderRegistrationCommand
        {
            get
            {
                if (_orderRegistrationCommand != null) return _orderRegistrationCommand;
                _orderRegistrationCommand = new RelayCommand<string>(ExecuteOrderRegistrationCommand, CanExecuteOrderRegistrationCommand);
                return _orderRegistrationCommand;
            }
        }
        public void ExecuteOrderRegistrationCommand(string arg)
        {
        }
        private bool CanExecuteOrderRegistrationCommand(string arg)
        {
            bool can = DisplayOrderManagementItems.Where(x => x.OrderItem.伝票ＮＯ == arg & x.RelationItem == null).Count() == 0;
            return can;
        }

        private ICommand _groupHeaderRefreshCommand;
        public ICommand GroupHeaderRefreshCommand
        {
            get
            {
                if (_groupHeaderRefreshCommand != null) return _groupHeaderRefreshCommand;
                _groupHeaderRefreshCommand = new RelayCommand<ReadOnlyObservableCollection<object>>(ExecuteGroupHeaderRefreshCommand);
                return _groupHeaderRefreshCommand;
            }
        }
        public void ExecuteGroupHeaderRefreshCommand(ReadOnlyObservableCollection<object> arg)
        {
            List<DisplayOrderManagementItem> items = new List<DisplayOrderManagementItem>();
            foreach (var a in arg)
            {
                items.Add((DisplayOrderManagementItem)a);
            }
            SyncronizeProduct(items);
        }

        private ICommand _groupHeaderAddEItemCommand;
        public ICommand GroupHeaderAddEItemCommand
        {
            get
            {
                if (_groupHeaderAddEItemCommand != null) return _groupHeaderAddEItemCommand;
                _groupHeaderAddEItemCommand = new RelayCommand<ReadOnlyObservableCollection<object>>(ExecuteGroupHeaderAddEItemCommand);
                return _groupHeaderAddEItemCommand;
            }
        }
        public void ExecuteGroupHeaderAddEItemCommand(ReadOnlyObservableCollection<object> arg)
        {
            List<DisplayOrderManagementItem> items = new List<DisplayOrderManagementItem>();
            foreach (var a in arg)
            {
                items.Add((DisplayOrderManagementItem)a);
            }
            EItemSelect(items);
        }

        private ICommand _selectedRelationDeleteCommand;
        public ICommand SelectedRelationDeleteCommand
        {
            get
            {
                if (_selectedRelationDeleteCommand != null) return _selectedRelationDeleteCommand;
                _selectedRelationDeleteCommand = new RelayCommand<object>(ExecuteSelectedRelationDeleteCommand, CanExecuteSelectedRelationDeleteCommand);
                return _selectedRelationDeleteCommand;
            }
        }
        public void ExecuteSelectedRelationDeleteCommand(object arg)
        {
            SelectedRelationItemDelete((DisplayOrderManagementItem)SelectedRowItem);
        }
        private bool CanExecuteSelectedRelationDeleteCommand(object arg)
        {
            return SelectedRowItem != null;
        }


        private void SelectedRelationItemDelete(DisplayOrderManagementItem targetItem)
        {
            UnReadObservables();

            if (targetItem.RelationItem == null) return;

            Relationship rItem = targetItem.RelationItem;

            //コンテキストから削除
            context.Entry(rItem).Reload();

            //表示用コレクションから削除
            var displayItems = DisplayOrderManagementItems.Where(x => x.RelationItem?.ProductId == rItem.ProductId & x.EItem?.ID == rItem.EItemId);
            foreach (var a in displayItems)
            {
                ForObservableChangesStatus = false;
                a.RelationItem = null;
                a.EItem = null;

                ForObservableChangesStatus = true;
            }

            if (context.Relationships.Where(x => x.ProductId == rItem.ProductId & x.EItemId == rItem.EItemId).Count() == 0)
            {
            }
            else
            {
                context.Relationships.Remove(rItem);
                context.SaveChanges();
            }

            RefreshDisplayRows();

            List<DisplayOrderManagementItem> SyncronizeItems = new List<DisplayOrderManagementItem>();
            var targetPitems = DisplayOrderManagementItems.Where(x => x.RelationItem?.ProductId == rItem.ProductId & x.OrderItem == targetItem.OrderItem);

            foreach (var a in targetPitems)
            {
                SyncronizeItems.Add(a);
            }

            if (SyncronizeItems.Count() != 0) SyncronizeProduct(SyncronizeItems);

            SetObservableProperties();
        }


        /// <summary>
        /// 空白行の整理を行う
        /// </summary>
        private void RefreshDisplayRows()
        {
            var test = DisplayOrderManagementItems.Select(x => x.OrderItem).Distinct().ToList();

            foreach (var oItem in test)
            {
                var FullItems = DisplayOrderManagementItems.Where(x => x.OrderItem == oItem & x.RelationItem != null);
                var NullItems = DisplayOrderManagementItems.Where(x => x.OrderItem == oItem & x.RelationItem == null);

                //ヌル行が1行だけの場合はスルー。
                if (FullItems.Count() == 0 & NullItems.Count() == 1)
                {
                }
                else
                {
                    //リレーションが設定されたアイテムを含んでいる場合
                    if (FullItems.Count() > 0 & NullItems.Count() > 0)
                    {
                        foreach (var a in NullItems.ToList())
                        {
                            DisplayOrderManagementItems.Remove(a);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < (NullItems.Count() - 1); i++)
                        {
                            DisplayOrderManagementItems.Remove(NullItems.First());
                        }
                    }
                }
            }

        }

        ///<summary>
        ///グループヘッダーから受け取った商品構成に対する同期処理
        /// </summary>
        private void SyncronizeProduct(List<DisplayOrderManagementItem> arg)
        {
            UnReadObservables();

            string productID = arg[0].ProductItem.ProductId.ToString();

            foreach (var a in arg)
            {
                var item = (DisplayOrderManagementItem)a;
                //受注アイテム
                if (item.OrderItem != null)
                {
                    var dbitem = context.Entry(item.OrderItem).GetDatabaseValues();
                    //受注アイテムがリアルタイムDBから削除されていた場合
                    if (dbitem == null)
                    {
                        context.Entry(item.OrderItem).Reload();
                        List<DisplayOrderManagementItem> dellItems = new List<DisplayOrderManagementItem>();
                        foreach (var b in DisplayOrderManagementItems.Where(x => x.OrderItem == item.OrderItem))
                        {
                            dellItems.Add(b);
                        }
                        foreach (var b in dellItems)
                        {
                            DisplayOrderManagementItems.Remove(b);
                        }
                    }
                    else
                    {
                        //受注状態フラグがNullの時はリターン
                        if (dbitem.GetValue<string>(nameof(OrderItem.OrderStatus)) != null)
                        {
                            context.Entry(item.OrderItem).Reload();
                            List<DisplayOrderManagementItem> dellItems = new List<DisplayOrderManagementItem>();
                            foreach (var b in DisplayOrderManagementItems.Where(x => x.OrderItem == item.OrderItem))
                            {
                                dellItems.Add(b);
                            }
                            foreach (var b in dellItems)
                            {
                                DisplayOrderManagementItems.Remove(b);
                            }
                        }
                    }
                }

                //リレーション
                if (item.RelationItem != null)
                {
                    var dbitem = context.Entry(item.RelationItem).GetDatabaseValues();
                    //リレーションがリアルタイムDBから削除されていた場合
                    if (dbitem == null)
                    {
                        context.Entry(item.RelationItem).Reload();
                        foreach (var b in DisplayOrderManagementItems.Where(x => x.RelationItem == item.RelationItem))
                        {
                            b.RelationItem = null;
                            b.EItem = null;
                        }
                    }
                    else
                    {
                        context.Entry(item.RelationItem).Reload();
                        context.Entry(item.EItem).Reload();
                    }
                }
            }

            var oldRelationItemsCache = context.Relationships.Local;
            List<Relationship> addRelations = new List<Relationship>();

            ///<summary>
            ///古いキャッシュに含まれていないアイテムを検出する。
            /// </summary>
            using (TechnoDB newContext = new TechnoDB())
            {
                foreach (var a in newContext.Relationships.ToList())
                {
                    var hitItem = oldRelationItemsCache.Where(x => x.ProductId == a.ProductId & x.EItemId == a.EItemId);

                    if (hitItem.Count() == 0)
                    {
                        addRelations.Add(a);
                    }
                }
            }

            ///<summary>
            ///表示用コレクションに含まれていないアイテムを検出する。
            /// </summary>
            foreach (var a in context.Relationships.Where(x => x.ProductId == productID).ToList())
            {
                if (DisplayOrderManagementItems.Where(x => x.ProductItem.ProductId == productID).Select(x => x.RelationItem).ToList().Contains(a) == false)
                {
                    if(addRelations.Where(x => x.ProductId == a.ProductId & x.EItemId == a.EItemId).Count() == 0)
                    {
                        addRelations.Add(a);
                    }
                }
            }

            UpdateContextCache();
            RefreshDisplayRows();

            ///<summary>
            ///リレーション情報の追加
            /// </summary>
            foreach (var vrItem in addRelations)
            {
                var targetOrderItems = DisplayOrderManagementItems.Where(x => x.ProductItem.ProductId == vrItem.ProductId).Select(x => x.OrderItem).Distinct();
                if (targetOrderItems.Count() == 0) break;
                List<DisplayOrderManagementItem> addItems = new List<DisplayOrderManagementItem>();

                foreach (var oItem in targetOrderItems.ToList())
                {
                    var count = DisplayOrderManagementItems.Count(x => x.OrderItem.伝票ＮＯ == oItem.伝票ＮＯ & x.RelationItem != null);

                    //対象となる構成情報に子がいない時
                    if (count == 0)
                    {
                        var targetItem = DisplayOrderManagementItems.Where(x => x.OrderItem.伝票ＮＯ == oItem.伝票ＮＯ & x.ProductItem.ProductId == vrItem.ProductId).First();
                        var eItem = context.EItems.Where(x => x.ID == vrItem.EItemId).First();
                        var rItem = context.Relationships.Where(x => x.ProductId == vrItem.ProductId & x.EItemId == vrItem.EItemId).First();

                        targetItem.RelationItem = rItem;
                        targetItem.EItem = eItem;
                    }
                    else
                    {
                        var addItem = new DisplayOrderManagementItem();
                        var eItem = context.EItems.Where(x => x.ID == vrItem.EItemId).First();
                        var pItem = context.ProductItems.Where(x => x.ProductId == vrItem.ProductId).First();
                        var rItem = context.Relationships.Where(x => x.ProductId == vrItem.ProductId & x.EItemId == vrItem.EItemId).First();

                        addItem.OrderItem = oItem;
                        addItem.ProductItem = pItem;
                        addItem.RelationItem = rItem;
                        addItem.EItem = eItem;

                        addItems.Add(addItem);
                        DisplayOrderManagementItems.Add(addItem);
                    }
                }
            }

            ResolveVendorItem();
            SetObservableProperties();
        }

        private bool CheckDatabaseChanged(List<DisplayOrderManagementItem> arg)
        {
            List<DisplayOrderManagementItem> localItems = new List<DisplayOrderManagementItem>();
            foreach (var a in arg)
            {
                localItems.Add((DisplayOrderManagementItem)a);
            }

            //構成の数が違ったらアウト
            string targetId = localItems[0].ProductItem.ProductId.ToString();
            using (TechnoDB newContext = new TechnoDB())
            {
                var dbItems = newContext.Relationships.Where(x => x.ProductId == targetId);
                if (localItems.Count(x => x.RelationItem != null) != dbItems.Count()) return true;
            }

            //アイテムの要素が違ったらアウト
            using (TechnoDB newContext = new TechnoDB())
            {
                newContext.OrderItems.ToList();
                newContext.Relationships.ToList();
                newContext.EItems.ToList();

                foreach (var localItem in localItems)
                {
                    //発注ステータスが違ったらアウト
                    if (localItem.OrderItem.OrderStatus != newContext.OrderItems.Local
                        .Where(x => x.伝票ＮＯ == localItem.OrderItem.伝票ＮＯ).Select(x => x.OrderStatus).First())
                    {
                        return true;
                    }

                    if (localItem.OrderItem.OrderStatus != newContext.OrderItems.Local
                        .Where(x => x.伝票ＮＯ == localItem.OrderItem.伝票ＮＯ).Select(x => x.OrderStatus).First())
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// 初期化
        /// </summary>
        private void Init()
        {
            context = null;
            collectionView = null;
            DisplayOrderManagementItems = null;
        }


        private void ResolveVendorItem()
        {
            UnReadObservables();
            foreach (var a in DisplayOrderManagementItems.Where(x => x.EItem != null))
            {
                if (a.EItem.Vender != a.VendorItem?.Id)
                {
                    var vItem = venderItems.Where(x => x.Id == a.EItem.Vender);
                    if (vItem.Count() != 0)
                    {
                        a.VendorItem = vItem.First();
                    }
                    else
                    {
                        a.EItem.Vender = null;
                        a.VendorItem = null;
                    }
                }
            }
            SetObservableProperties();
        }


        public TpicsDbContext tpicsContext = new TpicsDbContext();
        private DispatchObservableCollection<VendorItem> venderItems;

        /// <summary>
        /// 現在のインスタンスを新しいインスタンスで置き換える形でリロードする
        /// </summary>
        private async Task Load()
        {
            DialogContent = new LoadingTimeMessageDialog();
            venderItems = tpicsContext.Load();

            context = new TechnoDB();
            DisplayOrderManagementItems = new DispatchObservableCollection<DisplayOrderManagementItem>();

            IsDialogOpen = true;

            await Task.Run(() =>
            {
                context.OrderItems.ToList();
                context.ProductItems.ToList();
                context.EItems.ToList();
                context.Relationships.ToList();

                //BS受注のみが対象
                foreach (var oItem in context.OrderItems.Where(x => x.フラグ１ == "0" & x.OrderStatus == null))
                {
                    //商品コードはインポート時に必ず作成され、削除するコマンドも存在しない事を前提とする。
                    var pItem = context.ProductItems.Local.Where(x => x.ProductId == oItem.商品コード).First();
                    var children = context.Relationships.Local.Where(x => x.ProductId == oItem.商品コード);

                    //リレーションが存在しない場合、rItemとeItemはNULL
                    if (children.Count() == 0)
                    {
                        var AddItem = new DisplayOrderManagementItem();
                        AddItem.OrderItem = oItem;
                        AddItem.ProductItem = pItem;

                        DisplayOrderManagementItems.Add(AddItem);
                    }
                    else //リレーションが存在する場合
                    {
                        for (int i = 0; i < children.Count(); i++)
                        {
                            var AddItem = new DisplayOrderManagementItem();
                            AddItem.OrderItem = oItem;
                            AddItem.ProductItem = pItem;
                            AddItem.RelationItem = children.ToList()[i];
                            AddItem.EItem = context.EItems.Where(x => x.ID == AddItem.RelationItem.EItemId).First();
                            if(AddItem.EItem.Vender != null)
                            {
                                AddItem.VendorItem = venderItems.Where(x => x.Id == AddItem.EItem.Vender).First();
                            }

                            DisplayOrderManagementItems.Add(AddItem);
                        }
                    }
                }
            });

            SetObservableProperties();

            collectionView = CollectionViewSource.GetDefaultView(this.DisplayOrderManagementItems);
            PropertyGroupDescription aa = new PropertyGroupDescription("OrderItem.伝票ＮＯ");

            var d = new DrivedObject();
            await Task.Run(async () => 
            {
                if (!d.CheckAccess())
                {
                    await d.Dispatcher.InvokeAsync(() => collectionView.GroupDescriptions.Add(aa));
                }
                    
            });


        }


        private void SetObservableProperties()
        {
            //監視状態を設定 EItem
            foreach (var x in DisplayOrderManagementItems.Where(x => x.EItem != null).Select(x => x.EItem))
            {
                var a = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => x.PropertyChanged += h,
                    h => x.PropertyChanged -= h)
                    .Subscribe((System.Reactive.EventPattern<PropertyChangedEventArgs> e) =>
                    {
                        if (e.EventArgs.PropertyName == nameof(EItem.Vender))
                        {
                            var item = (DisplayOrderManagementItem)SelectedRowItem;

                            var vItem = venderItems.Where(y => y.Id == item.EItem.Vender);
                            if(vItem.Count() != 0)
                            {
                                item.VendorItem = vItem.First();
                            }
                            else
                            {
                                item.VendorItem = null;
                                item.EItem.Vender = null;
                            }

                        }
                        ObservablelSaveChanges();
                    });
                ObservableList.Add(a);
            }
            //リレーション
            foreach (var x in DisplayOrderManagementItems.Where(x => x.RelationItem != null).Select(x => x.RelationItem))
            {
                var a = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => x.PropertyChanged += h,
                    h => x.PropertyChanged -= h)
                    .Subscribe((System.Reactive.EventPattern<PropertyChangedEventArgs> e) =>
                    {
                        ObservablelSaveChanges();
                    });
                ObservableList.Add(a);
            }
        }

        private void UnReadObservables()
        {
            foreach(var a in ObservableList)
            {
                a.Dispose();
            }
        }

        private void ObservablelSaveChanges()
        {
            if (ForObservableChangesStatus == true)
            {
                context.SaveChanges();
            }
        }


        /// <summary>
        /// コンテキストのキャッシュを同期する
        /// </summary>
        private void UpdateContextCache()
        {
            context.OrderItems.ToList();
            context.ProductItems.ToList();
            context.Relationships.ToList();
            context.EItems.ToList();
        }


        /// <summary>
        /// Relationに加えるEItemを選択するダイアログを表示
        /// </summary>
        /// <param name="ProductID"></param>
        private void EItemSelect(List<DisplayOrderManagementItem> arg)
        {
            string ProductID = arg[0].ProductItem.ProductId.ToString();

            var DialogVM = new EItemSelectDialogVM();
            var DialogView = new EItemSelectDialog() { DataContext = DialogVM };
            DialogContent = DialogView;

            DialogView.DG1.PreviewMouseDoubleClick += (x, _) =>
            {
                if (DialogVM.SelectedEitem == null)
                {
                    return;
                }
                else
                {
                    YorNConfirmWindow ConfirmView = new YorNConfirmWindow();
                    ConfirmView.Message.Text = "[" + ProductID + "] に [" + DialogVM.SelectedEitem.ID + "] を加えますか？";
                    ConfirmView.AcceptBT.Click += (a, b) => { AddRelation(ProductID, DialogVM.SelectedEitem.ID, arg); };
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
                    ConfirmView.Message.Text = "[" + ProductID + "] に [" + DialogVM.SelectedEitem.ID + "] を加えますか？";
                    ConfirmView.AcceptBT.Click += (a, b) => { AddRelation(ProductID, DialogVM.SelectedEitem.ID, arg); };
                    ConfirmView.CancelBT.Click += (a, b) => { DialogContent = DialogView; };
                    DialogContent = ConfirmView;
                }
            };

            DialogView.CancelBT.Click += (x, _) => { IsDialogOpen = false; };
            IsDialogOpen = true;
        }


        /// <summary>
        /// リレーションアイテムを加える
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="eItemId"></param>
        /// <returns></returns>
        private bool AddRelation(string productID, string eItemId, List<DisplayOrderManagementItem> arg)
        {
            UnReadObservables();

            string msg = "";

            using (TechnoDB newContext = new TechnoDB())
            {
                var eItemCount = newContext.EItems.Where(x => x.ID == eItemId).Count();
                if (eItemCount == 0)
                {
                    msg = "選択されたEItemがDBに存在しません。";
                    AcceptConfirmDialog confirmDialog1 = new AcceptConfirmDialog();
                    confirmDialog1.Message.Text = msg;
                    confirmDialog1.AcceptBT.Click += (x, _) =>
                    {
                        IsDialogOpen = false;
                        SyncronizeProduct(arg);
                    };
                    DialogContent = confirmDialog1;
                    return false;
                }
                else
                {
                    var relationCount = newContext.Relationships.Where(x => x.ProductId == productID & x.EItemId == eItemId).Count();
                    if (relationCount > 0)
                    {
                        msg = "選択のアイテムは既に構成に含まれています。";
                        AcceptConfirmDialog confirmDialog2 = new AcceptConfirmDialog();
                        confirmDialog2.Message.Text = msg;
                        confirmDialog2.AcceptBT.Click += (x, _) =>
                        {
                            IsDialogOpen = false;
                            SyncronizeProduct(arg);
                        };
                        DialogContent = confirmDialog2;
                        return false;
                    }
                }
            }

            Relationship rItem = new Relationship() { ProductId = productID, EItemId = eItemId };
            context.Relationships.Add(rItem);
            context.SaveChanges();

            SyncronizeProduct(arg);
            IsDialogOpen = false;
            SetObservableProperties();
            return true;

        }
    }


    //VM用エンティティクラス
    public class DisplayOrderManagementItem : INotifyPropertyChanged, IEnumerable
    {
        private OrderItem _orderItem;
        private ProductItem _productItem;
        private Relationship _relationItem;
        private EItem _eItem;
        private VendorItem _vendorItem;

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
        public VendorItem VendorItem
        {
            get { return _vendorItem; }
            set
            {
                if (_vendorItem == value) return;
                _vendorItem = value;
                RaisePropertyChanged();
            }
        }

        public IEnumerator GetEnumerator()
        {
            yield return OrderItem;
            yield return ProductItem;
            yield return RelationItem;
            yield return EItem;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}