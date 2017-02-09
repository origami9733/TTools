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
                    if (String.IsNullOrEmpty(SendedFlag) == false && this.UpdateDate == null)
                        AddError(nameof(UpdateDate), "何日以降の情報を取得するか指定して下さい。");
                    else
                        RemoveError(nameof(UpdateDate), null);
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
        }
        #endregion

        #region プロパティ

        //検索条件を保持するプロパティ
        private string _sendedFlag = string.Empty;
        private DateTime? _updateDate = null;
        public string SendedFlag
        {
            get { return _sendedFlag; }
            set
            {
                if (_sendedFlag != value) _sendedFlag = value;
                ValidateProperty(nameof(UpdateDate), value);
            }
        }
        public DateTime? UpdateDate
        {
            get { return _updateDate; }
            set
            {
                if (_updateDate != value) _updateDate = value;
                ValidateProperty(nameof(UpdateDate), value);
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
        private DispatchObservableCollection<DisplayOrderItem> _displayOrderItems;
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
        public DispatchObservableCollection<DisplayOrderItem> DisplayOrderItems
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
        private bool _bsCategoryIsCheked;
        private bool _plCategoryIsCheked;
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
        public bool BsCategoryIsCheked
        {
            get { return _bsCategoryIsCheked; }
            set
            {
                if (_bsCategoryIsCheked == value) return;
                _bsCategoryIsCheked = value;
                RaisePropertyChanged();
            }
        }
        public bool PlCategoryIsCheked
        {
            get { return _plCategoryIsCheked; }
            set
            {
                if (_plCategoryIsCheked == value) return;
                _plCategoryIsCheked = value;
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

        #endregion

        #region 変数

        private TechnoDB context;
        private OrderItem orderItem = new OrderItem();
        private ICollectionView collectionView;

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
                foreach (DisplayOrderItem a in collectionView) a.IsChecked = true;
            }
            else
            {
                foreach (DisplayOrderItem a in DisplayOrderItems) a.IsChecked = false;
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

        private void ReloadAll()
        {
            this.IsDialogOpen = true;
            this.Init();

            //ロード中専用のダイログ画面をセット
            this.DialogContent = new LoadingTimeMessageDialog();

            DataLoad();

            this.MessageQueue = new SnackbarMessageQueue();
            string snackbarMessage;

            if (AllCategoryCount == 0) snackbarMessage = "新しいデータは見つかりませんでした。";
            else snackbarMessage = "データの取得に成功しました。";

            this.MessageQueue.Enqueue(snackbarMessage, "OK", () => IsSnackbarActive = false);

            this.AllCategoryIsChecked = true;
        } //データをリロードする。
        private async void DataLoad()
        {
            //非同期で読み込みメソッドをスタート
            await Task.Factory.StartNew(() =>
            {
                this.OriginalOrderItems = new DispatchObservableCollection<OrderItem>();
                this.OriginalOrderItems = this.orderItem.Load(MakeSQL());
            })
                .ContinueWith((t, _) =>
                {
                    this.DisplayOrderItems = new DispatchObservableCollection<DisplayOrderItem>();

                    //要素アイテムのイベント処理
                    foreach (var x in this.OriginalOrderItems)
                    {
                        DisplayOrderItem item = new DisplayOrderItem();

                        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                h => item.PropertyChanged += h,
                                h => item.PropertyChanged -= h)
                                .Subscribe(e =>
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
                }, null, TaskScheduler.FromCurrentSynchronizationContext());

            this.ReloadCounters();
            this.collectionView = CollectionViewSource.GetDefaultView(this.DisplayOrderItems) as ListCollectionView;
        }

        private async void ImportCheckedItems()
        {
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
                            MessageBox.Show("debug:キーが重複しています。");
                        }
                        else
                        {
                            this.context.OrderItems.Add(item.OrderItem);
                            successCount++;
                        }
                    }
                    this.context.SaveChanges();
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

            this.MessageQueue.Enqueue(snackbarMessage, "OK", () => IsSnackbarActive = false);
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
                        foreach (DisplayOrderItem a in DisplayOrderItems) a.IsChecked = false;
                        collectionView.Filter = null;
                        break;
                    case "Machine":
                        foreach (DisplayOrderItem a in DisplayOrderItems) a.IsChecked = false;
                        collectionView.Filter += x =>
                        {
                            var item = (DisplayOrderItem)x;
                            return item.OrderItem.フラグ１ == " ";
                        };
                        break;
                    case "BS":
                        foreach (DisplayOrderItem a in DisplayOrderItems) a.IsChecked = false;
                        collectionView.Filter += x =>
                        {
                            var item = (DisplayOrderItem)x;
                            return item.OrderItem.フラグ１ == "0";
                        };
                        break;
                    case "PL":
                        foreach (DisplayOrderItem a in DisplayOrderItems) a.IsChecked = false;
                        collectionView.Filter = x =>
                        {
                            var item = (DisplayOrderItem)x;
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
            BsCategoryIsCheked = false;
            PlCategoryIsCheked = false;
        } //カテゴリーの選択状態を解除する。

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
        } //受注データ取得用のSQL文を作成する。

        #endregion
    }

    //VM用エンティティクラス
    public class DisplayOrderItem : INotifyPropertyChanged
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

