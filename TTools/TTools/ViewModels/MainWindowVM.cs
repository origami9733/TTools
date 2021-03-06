﻿using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Deployment.Application;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TTools.Domain;
using TTools.EF;
using TTools.Models;
using TTools.Views;

namespace TTools.ViewModels
{
    public class MainWindowVM : INotifyPropertyChanged
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
        private int _selectedSideMenuIndex;
        #endregion

        public SideMenuItem[] SideMenuItems { get; }
        public int SelectedSideMenuIndex
        {
            get { return _selectedSideMenuIndex; }
            set
            {
                if (_selectedSideMenuIndex == value) return;
                _selectedSideMenuIndex = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowVM()
        {
            _selectedSideMenuIndex = 0;
            SideMenuItems = new[]
            {
                new SideMenuItem("インフォメーション",new Information()),
                new SideMenuItem("受注インポート",new ImportOrder(){DataContext = new ImportOrderVM(this) }),
                new SideMenuItem("BS受注管理",new BsOrdersManagement(){DataContext = new BsOrderManagementVM(this) }),
                new SideMenuItem("BS回答管理",new BsReplyManagement(){DataContext = new BsReplyManagementVM(this) }),
                new SideMenuItem("検収データI/O",new InspectionDataIO(){DataContext = new InspectionDataIOVM(this) }),
                new SideMenuItem("本体受注管理",new MachineOrderManagement(){DataContext = new MachineOrderManagementVM(this)}),
            };
        }


        
        private ICommand _openVersionConfirmDialogCommand;
        public ICommand OpenVersionConfirmDialogCommand
        {
            get
            {
                if (_openVersionConfirmDialogCommand != null) return _openVersionConfirmDialogCommand;
                _openVersionConfirmDialogCommand = new RelayCommand<object>(ExecuteOpenVersionConfirmDialogCommand);
                return _openVersionConfirmDialogCommand;
            }
        }
        public void ExecuteOpenVersionConfirmDialogCommand(object arg)
        {
            string ver = "Debug Mode";
            string update_date = "----/--/--";

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
                //**バージョン取得
                ver = "v" + ad.CurrentVersion.ToString();
                //**最終更新日取得
                update_date = ad.TimeOfLastUpdateCheck.ToString("yyyy/MM/dd");
            }
            var dialog = new YesConfirmDialog();

            dialog.Message.Text = ver + "\r\n" + update_date ;
            dialog.AcceptBT.Click += (x, y) => { IsDialogOpen = false; };
            DialogContent = dialog;
            IsDialogOpen = true;
        }
    }
}
