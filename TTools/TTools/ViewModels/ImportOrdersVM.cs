using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
    public class ImportOrderVM : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region プロパティの変更通知
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region エラー管理

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

        //エラー条件とエラーメッセージの指定
        protected void ValidateProperty(string propertyName, object value)
        {
            switch (propertyName)
            {
                case nameof(UpdateDate):
                    if (SelectedSendedFlag == receivedOrder && this.UpdateDate == null)
                        AddError(nameof(UpdateDate), @"日付を指定して下さい。");
                    else RemoveError(nameof(UpdateDate), null);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region コンストラクタ
        public ImportOrderVM(MainWindowVM arg)
        {
            masterVM = arg;
        }
        #endregion

        #region 変数
        private MainWindowVM masterVM;
        private TechnoDB context;
        private ICollectionView collectionView;
        private List<IDisposable> observableList = new List<IDisposable>();
        private const string noReceivedOrder = "No Recieved";
        private const string receivedOrder = "Recieved";
        #endregion

        #region プロパティ
        /// <summary>
        /// 伝送フラグの一覧
        /// </summary>
        public IList<string> SendedFlagItems
        {
            get
            {
                IList<string> items = new List<string>();
                items.Add(noReceivedOrder);
                items.Add(receivedOrder);
                return items;
            }
        }
        /// <summary>
        /// アップデート日
        /// </summary>
        private DateTime? _updateDate = null;
        public DateTime? UpdateDate
        {
            get { return _updateDate; }
            set
            {
                if (_updateDate == value) return;
                _updateDate = value;
                ValidateProperty(nameof(UpdateDate), value);
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 選択中の伝送フラグ
        /// </summary>
        private string _selectedSendedFlag = noReceivedOrder;
        public string SelectedSendedFlag
        {
            get { return _selectedSendedFlag; }
            set
            {
                if (_selectedSendedFlag == value) return;
                _selectedSendedFlag = value;
                ValidateProperty(nameof(UpdateDate), value);
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 選択中のカテゴリー
        /// </summary>
        private string _selectedCategory;
        public string SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                if (_selectedCategory == value) return;
                _selectedCategory = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 選択中のレコード
        /// </summary>
        private Object _selectedRowItem;
        public Object SelectedRowItem
        {
            get { return _selectedRowItem; }
            set
            {
                if (_selectedRowItem == value) return;
                _selectedRowItem = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 受注数カウンター関連
        /// </summary>
        public int? AllCount
        {
            get { return DisplayOrderItems?.Count(); }
        }
        public int? MachineCount
        {
            get { return DisplayOrderItems?.Count(x => x.OrderItem.フラグ１ == " "); }
        }
        public int? BsCount
        {
            get { return DisplayOrderItems?.Count(x => x.OrderItem.フラグ１ == "1"); }
        }
        public int? PlCount
        {
            get { return DisplayOrderItems?.Count(x => x.OrderItem.フラグ１ == "2"); }
        }
        /// <summary>
        /// カテゴリーの選択状態
        /// </summary>
        private bool _isAllChecked;
        public bool IsAllChecked
        {
            get { return _isAllChecked; }
            set
            {
                if (_isAllChecked == value) return;
                _isAllChecked = value;
                RaisePropertyChanged();
            }
        }
        private bool _isMachineChecked;
        public bool IsMachineChecked
        {
            get { return _isMachineChecked; }
            set
            {
                if (_isMachineChecked == value) return;
                _isMachineChecked = value;
                RaisePropertyChanged();
            }
        }
        private bool _isBsChecked;
        public bool IsBsChecked
        {
            get { return _isBsChecked; }
            set
            {
                if (_isBsChecked == value) return;
                _isBsChecked = value;
                RaisePropertyChanged();
            }
        }
        private bool _isPlChecked;
        public bool IsPlChecked
        {
            get { return _isPlChecked; }
            set
            {
                if (_isPlChecked == value) return;
                _isPlChecked = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// ヘッダーのチェックボックスの選択状態
        /// </summary>
        public bool IsCheckBoxHeaderChecked
        {
            get { return DisplayOrderItems?.Count(x => x.IsChecked == true) > 0; }
        }
        /// <summary>
        /// 選択状態のアイテムのカウント
        /// </summary>
        public string IsCheckedCount
        {
            get { return DisplayOrderItems?.Count(x => x.IsChecked == true).ToString(); }
        }
        /// <summary>
        /// スナックバー関連
        /// </summary>
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
        /// ダイアログ関連
        /// </summary>
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

        private DispatchObservableCollection<DisplayOrderImportItem> _displayOrderItems;
        public DispatchObservableCollection<DisplayOrderImportItem> DisplayOrderItems
        {
            get { return _displayOrderItems; }
            set
            {
                if (_displayOrderItems == value) return;
                _displayOrderItems = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        /// <summary>
        /// ロード
        /// </summary>
        private async Task Load()
        {
            UnReadObservables();

            context = new TechnoDB();
            context.ProductItems.ToList();
            DisplayOrderItems = new DispatchObservableCollection<DisplayOrderImportItem>();

            var ghContext = new BeautyGhContext();
            var originalOrderItems = new DispatchObservableCollection<OrderItem>();

            //商品コードマスタへの登録
            await Task.Run(() =>
            {
                originalOrderItems = ghContext.Load(MakeReadSQL());

                foreach (var oItem in originalOrderItems)
                {
                    if (context.ProductItems.Local.Count(x => x.ProductId == oItem.商品コード) == 0)
                    {
                        var AddItem = new ProductItem();
                        AddItem.ProductId = oItem.商品コード;
                        AddItem.AliasName = oItem.商品名;
                        AddItem.Detail = oItem.仕様_備考;
                        AddItem.BluePrint = oItem.業者図番;
                        AddItem.Price = oItem.発注単価;

                        switch (oItem.フラグ１)
                        {
                            case " ":
                                AddItem.Category = "Machine";
                                break;
                            case "0":
                                AddItem.Category = "BS";
                                break;
                            case "1":
                                AddItem.Category = "PL";
                                break;
                            case null:
                                break;
                            default:
                                break;
                        }
                        context.ProductItems.Add(AddItem);
                    }
                }
                context.SaveChanges();
            });

            //格納
            foreach (var x in originalOrderItems)
            {
                var item = new DisplayOrderImportItem();
                item.IsChecked = false;
                item.OrderItem = x;
                DisplayOrderItems.Add(item);
            }

            collectionView = CollectionViewSource.GetDefaultView(this.DisplayOrderItems) as ListCollectionView;
            SetObservable();
            ReloadOrderCounter();

            IsAllChecked = true;
            RaisePropertyChanged(nameof(ImportOrderVM.IsCheckBoxHeaderChecked));
            RaisePropertyChanged(nameof(ImportOrderVM.IsCheckedCount));
            IsDialogOpen = false;
        }
        /// <summary>
        /// 選択状態のレコードをインポートする
        /// </summary>
        private async Task ImportCheckedItems()
        {
            IsDialogOpen = true;
            DialogContent = new LoadingProgressDialog();
            int successCount = 0;

            await Task.Run(() =>
            {
                using (context = new TechnoDB())
                {
                    foreach (var item in DisplayOrderItems.Where(x => x.IsChecked == true))
                    {
                        if (context.OrderItems.Where(x => x.伝票ＮＯ == item.OrderItem.伝票ＮＯ).Count() == 0)
                        {
                            context.OrderItems.Add(item.OrderItem);
                            successCount++;
                        }
                    }
                    context.SaveChanges();
                }
            });

            await Load();

            string msg = successCount.ToString() + "件のデータのインポートに成功しました。";
            ShowSnackbar(msg);
        }
        /// <summary>
        /// 監視状態を設定する
        /// </summary>
        private void SetObservable()
        {
            //監視状態を設定 OrderItem
            foreach (var x in DisplayOrderItems)
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
        /// 監視プロパティに変更があった時に実行される
        /// </summary>
        /// <param name="e"></param>
        private void ChangeOrderItemProperty(EventPattern<PropertyChangedEventArgs> e)
        {
            switch (e.EventArgs.PropertyName)
            {
                case nameof(DisplayOrderImportItem.IsChecked):
                    RaisePropertyChanged(nameof(ImportOrderVM.IsCheckBoxHeaderChecked));
                    RaisePropertyChanged(nameof(ImportOrderVM.IsCheckedCount));
                    break;
            }
        }
        /// <summary>
        /// 受注数カウンターをリロードする
        /// </summary>
        private void ReloadOrderCounter()
        {
            RaisePropertyChanged(nameof(ImportOrderVM.AllCount));
            RaisePropertyChanged(nameof(ImportOrderVM.MachineCount));
            RaisePropertyChanged(nameof(ImportOrderVM.BsCount));
            RaisePropertyChanged(nameof(ImportOrderVM.PlCount));
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
        /// <summary>
        /// カテゴリーのフィルターを切り替える
        /// </summary>
        /// <param name="arg"></param>
        private void ChangeCategoryFilter(string arg)
        {
            if (SelectedCategory != arg)
            {
                switch (arg)
                {
                    case "ShowAll":
                        foreach (DisplayOrderImportItem a in DisplayOrderItems) a.IsChecked = false;
                        collectionView.Filter = null;
                        break;
                    case "Machine":
                        foreach (DisplayOrderImportItem a in DisplayOrderItems) a.IsChecked = false;
                        collectionView.Filter += x =>
                        {
                            var item = (DisplayOrderImportItem)x;
                            return item.OrderItem.フラグ１ == " ";
                        };
                        break;
                    case "BS":
                        foreach (DisplayOrderImportItem a in DisplayOrderItems) a.IsChecked = false;
                        collectionView.Filter += x =>
                        {
                            var item = (DisplayOrderImportItem)x;
                            return item.OrderItem.フラグ１ == "0";
                        };
                        break;
                    case "PL":
                        foreach (DisplayOrderImportItem a in DisplayOrderItems) a.IsChecked = false;
                        collectionView.Filter = x =>
                        {
                            var item = (DisplayOrderImportItem)x;
                            return item.OrderItem.フラグ１ == "1";
                        };
                        break;
                }
            }
        }
        /// <summary>
        /// カテゴリーを全て未選択にする
        /// </summary>
        private void UnSelectCategory()
        {
            IsAllChecked = false;
            IsMachineChecked = false;
            IsBsChecked = false;
            IsPlChecked = false;
        }
        /// <summary>
        /// データ取得用SQL文を発行する
        /// </summary>
        /// <returns></returns>
        private string MakeReadSQL()
        {
            string sql = "SELECT * FROM dbo.GH_0000101 WHERE 伝送フラグ = ";

            if (string.IsNullOrEmpty(SelectedSendedFlag)) sql = sql + "0 ";
            else
                switch (SelectedSendedFlag)
                {
                    case noReceivedOrder:
                        sql = sql + "0 ";
                        break;
                    case receivedOrder:
                        sql = sql + "1 ";
                        break;
                }

            if (UpdateDate != null) sql = string.Format(sql + "AND 更新日付 >= '{0}'", UpdateDate.Value.ToString("yyyy/MM/dd"));

            return sql;
        }


    
        private ICommand _loadCommand;
        public ICommand LoadCommand
        {
            get
            {
                if (_loadCommand == null)
                {
                    _loadCommand = new RelayCommand<object>(ExecuteLoadCommand, CanExecuteLoadCommand);
                }
                return _loadCommand;
            }
        }
        private async void ExecuteLoadCommand(object arg)
        {
            DialogContent = new LoadingProgressDialog();
            IsDialogOpen = true;

            await Load();

            if (AllCount == 0 || AllCount == null) ShowSnackbar("受注データは見つかりませんでした");
            else ShowSnackbar(AllCount.ToString() + "件のデータを取得しました。");
        }
        private bool CanExecuteLoadCommand(object arg)
        {
            if (_currentErrors.ContainsKey(nameof(UpdateDate)) == true) return false;
            return true;
        }

        private ICommand _changeCategoryCommand;
        public ICommand ChangeCategoryCommand
        {
            get
            {
                if (_changeCategoryCommand == null)
                {
                    _changeCategoryCommand = new RelayCommand<string>(ExecuteChangeCategoryCommand, CanExecuteChangeCategoryCommand);
                }
                return _changeCategoryCommand;
            }
        }
        private void ExecuteChangeCategoryCommand(string arg)
        {
            ChangeCategoryFilter(arg);
        }
        private bool CanExecuteChangeCategoryCommand(string arg)
        {
            return DisplayOrderItems != null;
        }

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
                foreach (DisplayOrderImportItem a in collectionView) a.IsChecked = true;
            }
            else
            {
                foreach (DisplayOrderImportItem a in DisplayOrderItems) a.IsChecked = false;
            }
        }
        private bool CanExecuteCheckAllCommand(object arg)
        {
            return DisplayOrderItems != null;
        }

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
        public void ExecuteImportChekedItemsCommand(string arg)
        {
            var dialog = new YesNoConfirmDialog();
            dialog.Message.Text = "選択中の" + IsCheckedCount + "件のアイテムを取り込みます。";
            dialog.AcceptBT.Click += async (x, _) =>
            {
                await ImportCheckedItems();
                IsDialogOpen = false;
            };
            dialog.CancelBT.Click += (x, _) =>
            {
                IsDialogOpen = false;
            };

            DialogContent = dialog;
            IsDialogOpen = true;
        }
        private bool CanExecuteImportCheckedItemCommand(string arg)
        {
            return DisplayOrderItems?.Count(x => x.IsChecked == true) > 0;
        }
    }
}