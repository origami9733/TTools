using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
    public class MachineOrderManagementVM : INotifyPropertyChanged, INotifyDataErrorInfo
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
        private ICollectionView collectionView;
        #endregion

        private object _selectedRowItem;
        public object SelectedRowItem
        {
            get { return _selectedRowItem; }
            set
            {
                if (_selectedRowItem == value) return;
                _selectedRowItem = value;
                RaisePropertyChanged();
            }
        }
        private DispatchObservableCollection<DisplayMachineOrderManagementItem> _displayItems;
        public DispatchObservableCollection<DisplayMachineOrderManagementItem> DisplayItems
        {
            get { return _displayItems; }
            set
            {
                if (_displayItems == value) return;
                _displayItems = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="arg"></param>
        public MachineOrderManagementVM(MainWindowVM arg)
        {
            masterVM = arg;
        }
        /// <summary>
        /// ロード
        /// </summary>
        public async void Load()
        {
            context = new TechnoDB();
            context.OrderItems.ToList();
            context.ProductItems.ToList();
            DisplayItems = new DispatchObservableCollection<DisplayMachineOrderManagementItem>();
            BindingOperations.EnableCollectionSynchronization(DisplayItems, new object());

            var dialog = new LoadingProgressDialog();
            DialogContent = dialog;
            IsDialogOpen = true;

            await Task.Run(() =>
            {
                foreach (var oItem in context.OrderItems.Where(x => x.フラグ１ == " " && x.OrderStatus == null))
                {
                    var pItem = context.ProductItems.Where(x => x.ProductId == oItem.商品コード)?.First();

                    var addItem = new DisplayMachineOrderManagementItem();
                    addItem.OrderItem = oItem;
                    addItem.ProductItem = pItem;

                    DisplayItems.Add(addItem);
                }
            });

            collectionView = CollectionViewSource.GetDefaultView(DisplayItems);
            collectionView.SortDescriptions.Add(new SortDescription("OrderItem.契約番号", ListSortDirection.Ascending));
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("OrderItem.契約番号"));
            IsDialogOpen = false;
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
            Load();
        }
    }
}
