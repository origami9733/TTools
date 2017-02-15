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
using Reactive.Bindings;
using System.Reactive.Linq;
using System.Windows.Controls;

namespace TTools.ViewModels
{
    public class ImportOrdersVM : INotifyPropertyChanged, INotifyDataErrorInfo
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
                    if (SelectedSendedFlag== receivedOrder && this.UpdateDate == null)
                        AddError(nameof(UpdateDate), @"日付を指定して下さい。");
                    else RemoveError(nameof(UpdateDate), null);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region コンストラクタ
        public ImportOrdersVM()
        {
            Init();

            SendedFlagList = new List<string>();
            SendedFlagList.Add(noReceivedOrder);
            SendedFlagList.Add(receivedOrder);
            SelectedSendedFlag = noReceivedOrder;
        }
        #endregion

        #region プロパティ
        //検索条件を保持するプロパティ
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
        //選択中の情報を保持するプロパティ
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
        //コレクションを保持するプロパティ
        private DispatchObservableCollection<OrderItem> _originalOrderItems;
        private DispatchObservableCollection<DisplayImportOrderItem> _displayOrderItems;
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
        public DispatchObservableCollection<DisplayImportOrderItem> DisplayOrderItems
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
        //受注数のカウントを保持するプロパティ
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
        //カテゴリーの選択状態を保持するプロパティ
        private bool _allCategoryIsChecked;
        private bool _machineCategoryIsChecked;
        private bool _bsCategoryIsChecked;
        private bool _plCategoryIsChecked;
        public bool AllCategoryIsChecked
        {
            get { return _allCategoryIsChecked; }
            set
            {
                if (_allCategoryIsChecked == value) return;
                _allCategoryIsChecked = value;
                RaisePropertyChanged();
            }
        }
        public bool MachineCategoryIsChecked
        {
            get { return _machineCategoryIsChecked; }
            set
            {
                if (_machineCategoryIsChecked == value) return;
                _machineCategoryIsChecked = value;
                RaisePropertyChanged();
            }
        }
        public bool BsCategoryIsChecked
        {
            get { return _bsCategoryIsChecked; }
            set
            {
                if (_bsCategoryIsChecked == value) return;
                _bsCategoryIsChecked = value;
                RaisePropertyChanged();
            }
        }
        public bool PlCategoryIsChecked
        {
            get { return _plCategoryIsChecked; }
            set
            {
                if (_plCategoryIsChecked == value) return;
                _plCategoryIsChecked = value;
                RaisePropertyChanged();
            }
        }
        //チェックボックスヘッダーの状態を保持するプロパティ
        public bool CheckBoxColumnHeaderIsChecked
        {
            get { return DisplayOrderItems?.Count(x => x.IsChecked == true) > 0; }
        }
        //チェック時に表示するメッセージを保持するプロパティ
        public string IsCheckedMessage
        {
            get { return DisplayOrderItems?.Count(x => x.IsChecked == true).ToString(); }
        }
        //スナックバー関連
        private bool _isSnackbarActive;
        private SnackbarMessageQueue _messageQueue;
        public bool IsSnackbarActive
        {
            get { return _isSnackbarActive; }
            set
            {
                if (_isSnackbarActive != value)
                {
                    _isSnackbarActive = value;
                    RaisePropertyChanged();
                }
            }
        }
        public SnackbarMessageQueue MessageQueue
        {
            get { return _messageQueue; }
            set
            {
                if (_messageQueue != value)
                {
                    _messageQueue = value;
                    RaisePropertyChanged();
                }
            }
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
        //伝送フラグ関連
        public IList<string> SendedFlagList { get; set; }
        private string _selectedSendedFlag;
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
        #endregion

        #region 変数
        private TechnoDB context;
        private OrderItem orderItem = new OrderItem();
        private ICollectionView collectionView;
        private const string noReceivedOrder= "No Recieved";
        private const string receivedOrder = "Recieved";
        #endregion

        #region コマンド
        private ICommand _reloadCommand;
        private ICommand _changeCategoryCommand;
        private ICommand _checkAllCommand;
        private ICommand _importCheckedItemsCommand;
        //データのリロードコマンド
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
        private void ExecuteReloadCommand(String arg)
        {
            ReloadAll();
        }
        private bool CanExecuteReloadCommand(string arg)
        {
            if (_currentErrors.ContainsKey(nameof(UpdateDate)) == true) return false;
            return true;
        }
        //カテゴリーの切り替えコマンド
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
            ChangeCategory(arg);
        }
        private bool CanExecuteChangeCategoryCommand(string arg)
        {
            return DisplayOrderItems != null;
        }
        //チェックボックスヘッダーの操作コマンド
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
                foreach (DisplayImportOrderItem a in collectionView) a.IsChecked = true;
            }
            else
            {
                foreach (DisplayImportOrderItem a in DisplayOrderItems) a.IsChecked = false;
            }
        }
        private bool CanExecuteCheckAllCommand(object arg)
        {
            return DisplayOrderItems != null;
        }
        //インポートを実行するコマンド
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
            ImportCheckedItems();
        }
        private bool CanExecuteImportCheckedItemCommand(string arg)
        {
            return DisplayOrderItems?.Count(x => x.IsChecked == true) > 0;
        }
        #endregion

        #region メソッド
        private void Init()
        {
            UnSelectCategory();
            CounterClear();

            context = null;
            collectionView = null;

            DisplayOrderItems = null;
            OriginalOrderItems = null;
        } //全て初期化する。
        private async void DataLoad()
        {
            context = new TechnoDB();

            //非同期で読み込みメソッドをスタート
            await Task.Factory.StartNew(() =>
            {
                OriginalOrderItems = new DispatchObservableCollection<OrderItem>();
                OriginalOrderItems = orderItem.Load(MakeSQL());
                context.ProductItems.ToList();

                ///<summary>
                ///取得した受注データの中に未登録の商品コードが存在した場合はその場で登録を行う。
                ///</summary>
                foreach(var oItem in OriginalOrderItems)
                {
                    if (context.ProductItems.Local.Count(x => x.ProductId == oItem.商品コード) == 0)
                    {
                        ProductItem AddItem = new ProductItem();
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
                        }

                        context.ProductItems.Add(AddItem);

                        AddItem = null;
                    }
                }
                context.SaveChanges();
            })
                .ContinueWith((Action<Task, object>)((t, _) =>
                {
                    this.DisplayOrderItems = new DispatchObservableCollection<DisplayImportOrderItem>();

                    //要素アイテムのイベント処理
                    foreach (var x in this.OriginalOrderItems)
                    {
                        DisplayImportOrderItem item = new DisplayImportOrderItem();

                        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                h => item.PropertyChanged += h,
                                h => item.PropertyChanged -= h)
                                .Subscribe((System.Reactive.EventPattern<PropertyChangedEventArgs> e) =>
                                {
                                    // アイテムに変更があったら下記を実行する。
                                    RaisePropertyChanged("CheckBoxColumnHeaderIsChecked");
                                    RaisePropertyChanged("IsCheckedMessage");
                                });

                        item.IsChecked = false;
                        item.OrderItem = x;
                        this.DisplayOrderItems.Add(item);
                    }
                    IsDialogOpen = false;
                }), null, TaskScheduler.FromCurrentSynchronizationContext());

            this.ReloadCounters();
            this.collectionView = CollectionViewSource.GetDefaultView(this.DisplayOrderItems) as ListCollectionView;
        }
        private void ReloadAll()
        {
            this.IsDialogOpen = true;
            this.Init();

            //ロード中専用のダイログ画面をセット
            this.DialogContent = new LoadingTimeMessageDialog();

            DataLoad();

            this.MessageQueue = new SnackbarMessageQueue();
            string snackbarMessage;

            if (AllCategoryCount == 0 || AllCategoryCount == null) snackbarMessage = "受注データが見つかりませんでした。";
            else snackbarMessage = AllCategoryCount.ToString() + "件のデータが見つかりました。";

            this.MessageQueue.Enqueue(snackbarMessage, "確認", () => IsSnackbarActive = false);
            this.AllCategoryIsChecked = true;
        } //データをリロードする。     
        private async void ImportCheckedItems()
        {

            var sampleMessageDialog = new DefaultMessageDialog
            {
                Message = { Text = "選択中の" + IsCheckedMessage + "件のアイテムを取り込みます。" },
            };
            await DialogHost.Show(sampleMessageDialog, "RootDialog");

            if (sampleMessageDialog.Accept == false)
            {
                return;
            }

            this.IsDialogOpen = true;
            this.DialogContent = new LoadingTimeMessageDialog();
            int successCount = 0;

            await Task.Factory.StartNew(() =>
            {
                using (context = new TechnoDB())
                {
                    foreach (var item in this.DisplayOrderItems.Where(x => x.IsChecked == true))
                    {
                        if (this.context.OrderItems.Count(x => x.伝票ＮＯ == item.OrderItem.伝票ＮＯ) > 0)
                        {
                            MessageBox.Show("exeption message : contain key include ");
                        }
                        else
                        {
                            this.context.OrderItems.Add(item.OrderItem);
                            successCount++;
                        }
                    }

                    try
                    {
                        this.context.SaveChanges();
                    }
                    catch
                    {
                        return;
                    }
                }
            })
            .ContinueWith((t, _) =>
            {
                IsDialogOpen = false;
            }, null, TaskScheduler.FromCurrentSynchronizationContext());

            DataLoad();

            this.MessageQueue = new SnackbarMessageQueue();
            string snackbarMessage;

            snackbarMessage = successCount.ToString() + "件のデータを取り込みに成功しました。";

            this.MessageQueue.Enqueue(snackbarMessage, "確認", () => IsSnackbarActive = false);
            this.AllCategoryIsChecked = true;
        }

        private void ReloadCounters()
        {
            if (DisplayOrderItems?.Count() != null) AllCategoryCount = DisplayOrderItems.Count();
            else AllCategoryCount = null;

            if (DisplayOrderItems?.Where(x => x.OrderItem.フラグ１ == " ").Count() != null)
                MachineCategoryCount = DisplayOrderItems.Where(x => x.OrderItem.フラグ１ == " ").Count();
            else MachineCategoryCount = null;

            if (DisplayOrderItems?.Where(x => x.OrderItem.フラグ１ == "0").Count() != null)
                BsCategoryCount = DisplayOrderItems?.Where(x => x.OrderItem.フラグ１ == "0").Count();
            else BsCategoryCount = null;

            if (DisplayOrderItems?.Where(x => x.OrderItem.フラグ１ == "1").Count() != null)
                PlCategoryCount = DisplayOrderItems?.Where(x => x.OrderItem.フラグ１ == "1").Count();
            else PlCategoryCount = null;
        } //受注数カウンターを再計算する。
        private void CounterClear()
        {
            AllCategoryCount = null;
            MachineCategoryCount = null;
            BsCategoryCount = null;
            PlCategoryCount = null;
        } //受注数カウンターのクリア。

        private void ChangeCategory(string arg)
        {
            if (SelectedCategory != arg)
            {
                switch (arg)
                {
                    case "ShowAll":
                        foreach (DisplayImportOrderItem a in DisplayOrderItems) a.IsChecked = false;
                        collectionView.Filter = null;
                        break;
                    case "Machine":
                        foreach (DisplayImportOrderItem a in DisplayOrderItems) a.IsChecked = false;
                        collectionView.Filter += x =>
                        {
                            var item = (DisplayImportOrderItem)x;
                            return item.OrderItem.フラグ１ == " ";
                        };
                        break;
                    case "BS":
                        foreach (DisplayImportOrderItem a in DisplayOrderItems) a.IsChecked = false;
                        collectionView.Filter += x =>
                        {
                            var item = (DisplayImportOrderItem)x;
                            return item.OrderItem.フラグ１ == "0";
                        };
                        break;
                    case "PL":
                        foreach (DisplayImportOrderItem a in DisplayOrderItems) a.IsChecked = false;
                        collectionView.Filter = x =>
                        {
                            var item = (DisplayImportOrderItem)x;
                            return item.OrderItem.フラグ１ == "1";
                        };
                        break;
                }
            }
        } //選択中のカテゴリーを切り替える。
        private void UnSelectCategory()
        {
            AllCategoryIsChecked = false;
            MachineCategoryIsChecked = false;
            BsCategoryIsChecked = false;
            PlCategoryIsChecked = false;
        } //カテゴリーの選択状態を解除する。

        private string MakeSQL()
        {
            string sql = "SELECT * FROM dbo.GH_0000101 WHERE 伝送フラグ = ";

            if (string.IsNullOrEmpty(SelectedSendedFlag))
                sql = sql + "0 ";
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

            if (UpdateDate != null)
                sql = string.Format(sql + "AND 更新日付 >= '{0}'", UpdateDate.Value.ToString("yyyy/MM/dd"));

            Debug.Print("★SQL発行： " + sql);

            return sql;
        } //受注データ取得用のSQL文を作成する。
        #endregion
    }

    //VM用エンティティクラス
    public class DisplayImportOrderItem : INotifyPropertyChanged
    {
        private bool _isChecked;
        private OrderItem _orderItem;
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}