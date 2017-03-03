using MaterialDesignThemes.Wpf;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TTools.Domain;
using TTools.Properties;
using TTools.Views;

namespace TTools.ViewModels
{
    public class InspectionDataIOVM : INotifyPropertyChanged, INotifyDataErrorInfo
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
        public IEnumerable GetErrors(string propertyName)
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
                case nameof(ExportSourceFolder):
                    if (new DirectoryInfo(ExportSourceFolder).Exists == false) AddError(nameof(ExportSourceFolder), @"指定のディレクトリは存在しません。");
                    else RemoveError(nameof(ExportSourceFolder), null);
                    break;
                case nameof(ExportCsvFolder):
                    if (new DirectoryInfo(ExportCsvFolder).Exists == false) AddError(nameof(ExportCsvFolder), @"指定のディレクトリは存在しません");
                    else RemoveError(nameof(ExportCsvFolder), null);
                    break;
                case nameof(ExportXlsxFolder):
                    if (new DirectoryInfo(ExportXlsxFolder).Exists == false) AddError(nameof(ExportXlsxFolder), @"指定のディレクトリは存在しません");
                    else RemoveError(nameof(ExportXlsxFolder), null);
                    break;
                case nameof(ExportSourceFileName):
                    if (ExportSourceFileName.Substring(ExportSourceFileName.Length - 4, 4) != ".csv") AddError(nameof(ExportSourceFileName), @"指定のファイル名は正しくありません。");
                    else RemoveError(nameof(ExportSourceFileName), null);
                    break;
                case nameof(ExportCsvFileName):
                    if (ExportCsvFileName.Substring(ExportCsvFileName.Length - 4, 4) != ".csv") AddError(nameof(ExportCsvFileName), @"指定のファイル名は正しくありません。");
                    else RemoveError(nameof(ExportCsvFileName), null);
                    break;
                case nameof(ExportXlsxFileName):
                    if (ExportXlsxFileName.Substring(ExportXlsxFileName.Length - 4, 4) != ".xlsx") AddError(nameof(ExportXlsxFileName), @"指定のファイル名は正しくありません。");
                    else RemoveError(nameof(ExportXlsxFileName), null);
                    break;
                case nameof(ExportSourceFileExist):
                    if (ExportSourceFileExist == notExist) AddError(nameof(ExportSourceFileExist), "ソースが存在しません。");
                    else RemoveError(nameof(ExportSourceFileExist), null);
                    break;
                case nameof(ImportSql):
                    if (string.IsNullOrEmpty(ImportSql) == true)
                    {
                        AddError(nameof(ImportSql), "SQLはNullを許可しません。");
                    }
                    else RemoveError(nameof(ImportSql), null);
                    break;

                case nameof(ImportCsvFolder):
                    if (new DirectoryInfo(ImportCsvFolder).Exists == false) AddError(nameof(ImportCsvFolder), @"指定のディレクトリは存在しません");
                    else RemoveError(nameof(ImportCsvFolder), null);
                    break;
                case nameof(ImportCsvFileName):
                    if (ImportCsvFileName.Substring(ImportCsvFileName.Length - 4, 4) != ".csv") AddError(nameof(ImportCsvFileName), @"指定のファイル名は正しくありません。");
                    else RemoveError(nameof(ImportCsvFileName), null);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region ローカル変数
        private MainWindowVM masterVM;
        private string notExist = "NONE";
        #endregion

        public string ExportSourceFolder
        {
            get { return Settings.Default.InspectionExportSourceFolder; }
            set
            {
                var newValue = CheckBackSlash(value);
                if (Settings.Default.InspectionExportSourceFolder == newValue) return;
                Settings.Default.InspectionExportSourceFolder = newValue;
                Settings.Default.Save();
                ValidateProperty(nameof(ExportSourceFolder), newValue);
                RaisePropertyChanged();
            }
        }
        public string ExportSourceFileName
        {
            get { return Settings.Default.InspectionExportSourceFileName; }
            set
            {
                if (Settings.Default.InspectionExportSourceFileName == value) return;
                Settings.Default.InspectionExportSourceFileName = value;
                Settings.Default.Save();
                ValidateProperty(nameof(ExportSourceFileName), value);
                RaisePropertyChanged();
            }
        }
        public string ExportXlsxFolder
        {
            get { return Settings.Default.InspectionExportXlsxFolder; }
            set
            {
                var newValue = CheckBackSlash(value);
                if (Settings.Default.InspectionExportXlsxFolder == newValue) return;
                Settings.Default.InspectionExportXlsxFolder = newValue;
                Settings.Default.Save();
                ValidateProperty(nameof(ExportXlsxFolder), newValue);
                RaisePropertyChanged();
            }
        }
        public string ExportXlsxFileName
        {
            get { return Settings.Default.InspectionExportXlsxFileName; }
            set
            {
                if (Settings.Default.InspectionExportXlsxFileName == value) return;
                Settings.Default.InspectionExportXlsxFileName = value;
                Settings.Default.Save();
                ValidateProperty(nameof(ExportXlsxFileName), value);
                RaisePropertyChanged();
            }
        }
        public string ExportCsvFolder
        {
            get { return Settings.Default.InspectionExportCsvFolder; }
            set
            {
                var newValue = CheckBackSlash(value);
                if (Settings.Default.InspectionExportCsvFolder == newValue) return;
                Settings.Default.InspectionExportCsvFolder = newValue;
                Settings.Default.Save();
                ValidateProperty(nameof(ExportCsvFolder), newValue);
                RaisePropertyChanged();
            }
        }
        public string ExportCsvFileName
        {
            get { return Settings.Default.InspectionExportCsvFileName; }
            set
            {
                if (Settings.Default.InspectionExportCsvFileName == value) return;
                Settings.Default.InspectionExportCsvFileName = value;
                Settings.Default.Save();
                ValidateProperty(nameof(ExportCsvFileName), value);
                RaisePropertyChanged();
            }
        }

        public string ExportSourceFileExist
        {
            get
            {
                string returnStr;
                var file = new FileInfo(ExportSourceFolder + ExportSourceFileName);
                if (!file.Exists) returnStr = notExist;
                else returnStr = file.Name;
                return returnStr;
            }
        }
        public string ExportXlsxFileExist
        {
            get
            {
                string returnStr;
                var file = new FileInfo(ExportXlsxFolder + ExportXlsxFileName);
                if (!file.Exists) returnStr = notExist;
                else returnStr = file.Name;
                return returnStr;
            }
        }
        public string ExportCsvFileExist
        {
            get
            {
                string returnStr;
                var file = new FileInfo(ExportCsvFolder + ExportCsvFileName);
                if (!file.Exists) returnStr = notExist;
                else returnStr = file.Name;
                return returnStr;
            }
        }

        public string ExportSourceFileLastDate
        {
            get
            {
                string returnStr;
                var file = new FileInfo(ExportSourceFolder + ExportSourceFileName);
                if (!file.Exists) returnStr = "----/--/--";
                else returnStr = file.LastWriteTime.ToString("yyyy/MM/dd   hh:mm:ss");
                return returnStr;
            }
        }
        public string ExportXlsxFileLastDate
        {
            get
            {
                string returnStr;
                var file = new FileInfo(ExportXlsxFolder + ExportXlsxFileName);
                if (!file.Exists) returnStr = "----/--/--";
                else returnStr = file.LastWriteTime.ToString("yyyy/MM/dd   hh:mm:ss");
                return returnStr;
            }
        }
        public string ExportCsvFileLastDate
        {
            get
            {
                string returnStr;
                var file = new FileInfo(ExportCsvFolder + ExportCsvFileName);
                if (!file.Exists) returnStr = "----/--/--";
                else returnStr = file.LastWriteTime.ToString("yyyy/MM/dd   hh:mm:ss");
                return returnStr;
            }
        }

        public string ImportSql
        {
            get { return @Settings.Default.InspectionImportSql; }
            set
            {
                if (Settings.Default.InspectionImportSql == value) return;
                Settings.Default.InspectionImportSql = value;
                Settings.Default.Save();
                ValidateProperty(nameof(ImportSql), value);
                RaisePropertyChanged();
            }
        }

        public string ImportCsvFolder
        {
            get { return Settings.Default.InspectionImportCsvFolder; }
            set
            {
                var newValue = CheckBackSlash(value);
                if (Settings.Default.InspectionImportCsvFolder == newValue) return;
                Settings.Default.InspectionImportCsvFolder = newValue;
                Settings.Default.Save();
                ValidateProperty(nameof(ImportCsvFolder), newValue);
                RaisePropertyChanged();
            }
        }
        public string ImportCsvFileName
        {
            get { return Settings.Default.InspectionImportCsvFileName; }
            set
            {
                if (Settings.Default.InspectionImportCsvFileName == value) return;
                Settings.Default.InspectionImportCsvFileName = value;
                Settings.Default.Save();
                ValidateProperty(nameof(ImportCsvFileName), value);
                RaisePropertyChanged();
            }
        }

        public string ImportCsvFileExist
        {
            get
            {
                string returnStr;
                var file = new FileInfo(ImportCsvFolder + ImportCsvFileName);
                if (!file.Exists) returnStr = notExist;
                else returnStr = file.Name;
                return returnStr;
            }
        }
        public string ImportCsvFileLastDate
        {
            get
            {
                string returnStr;
                var file = new FileInfo(ImportCsvFolder + ImportCsvFileName);
                if (!file.Exists) returnStr = "----/--/--";
                else returnStr = file.LastWriteTime.ToString("yyyy/MM/dd   hh:mm:ss");
                return returnStr;
            }
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="arg"></param>
        public InspectionDataIOVM(MainWindowVM arg)
        {
            masterVM = arg;
        }
        /// <summary>
        /// ディレクトリパスにバックスラッシュを挿入する
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private string CheckBackSlash(string arg)
        {
            var returnStr = arg;

            if (returnStr.Substring(arg.Length - 1, 1) != @"\")
            {
                returnStr = returnStr + @"\";
            }

            return returnStr;
        }
        /// <summary>
        /// ソースファイルの存在確認
        /// </summary>
        private void ReloadFilesExist()
        {
            RaisePropertyChanged(nameof(ExportSourceFileExist));
            RaisePropertyChanged(nameof(ExportSourceFileLastDate));
            RaisePropertyChanged(nameof(ExportXlsxFileExist));
            RaisePropertyChanged(nameof(ExportXlsxFileLastDate));
            RaisePropertyChanged(nameof(ExportCsvFileExist));
            RaisePropertyChanged(nameof(ExportCsvFileLastDate));

            RaisePropertyChanged(nameof(ImportCsvFileExist));
            RaisePropertyChanged(nameof(ImportCsvFileLastDate));

        }
        /// <summary>
        /// ファイルを開く
        /// </summary>
        /// <param name="filePath"></param>
        private void FileOpen(string filePath)
        {
            try
            {
                Process.Start(filePath);
            }
            catch
            {
                var dialog = new YesConfirmDialog();
                dialog.Message.Text = "ファイルが見つかりませんでした。";
                dialog.AcceptBT.Click += (x, y) => { IsDialogOpen = false; };
                DialogContent = dialog;
                IsDialogOpen = true;
            }
        }
        /// <summary>
        /// ファイル上書きモードでエクスポートデータ作成を実行
        /// </summary>
        private async void ExportWhithinOverWrite()
        {
            DialogContent = new LoadingProgressDialog();
            IsDialogOpen = true;
            await Task.Run(() =>
            {
                //ファイルの存在確認
                var newCsv = new FileInfo(ExportCsvFolder + ExportCsvFileName);
                if (newCsv.Exists)
                {
                    newCsv.Delete();
                    newCsv = new FileInfo(ExportCsvFolder + ExportCsvFileName);
                }
                var newXlsx = new FileInfo(ExportXlsxFolder + ExportXlsxFileName);
                if (newXlsx.Exists)
                {
                    newXlsx.Delete();
                    newXlsx = new FileInfo(ExportXlsxFolder + ExportXlsxFileName);
                }

                using (ExcelPackage package = new ExcelPackage(newXlsx))
                {
                    //CSV用シート作成
                    ExcelWorksheet csv = package.Workbook.Worksheets.Add("CSV");

                    //CSVインポート
                    var format = new ExcelTextFormat();
                    format.TextQualifier = '"';
                    format.SkipLinesBeginning = 0;
                    format.SkipLinesEnd = 0;
                    format.Encoding = Encoding.GetEncoding("Shift-JIS");
                    csv.Cells[1, 1].LoadFromText(new FileInfo(ExportSourceFolder + ExportSourceFileName), format);

                    //変換用シート作成
                    ExcelWorksheet target = package.Workbook.Worksheets.Add("sheet1");

                    //最終入力行検出
                    int endRow = csv.Dimension.End.Row;

                    //ヘッダ文字列の設定
                    string[] headers =
                    {
                    "PORDER","PEDA","KBAN","BUN","CODE","BUMO","VENDOR","WMAN","AKUBU","AKUBU2",
                    "JITU","JITU0","IDATE","BDATE","BTIME","FDATE","FTIME","ADTIME","ATIME","VMTIME",
                    "SETTACT","TAX","RATE","CRATE","LOTNAME","NOTE","KEIRI","APRICE","KOUNYUUGAKU",
                    "WCODE","REV","VCODE","HOKAN","MINASIZAIK","HVOL","TUVOL","KARIUVOL","HAIKI",
                    "FURYOUVOL","DOSEIBAN","DORIREKIOYA","DORIREKIKO","WATRN","GDATE","INPUTDATE","INPUTUSER"};

                    //ヘッダの挿入
                    for (int i = 0; i < headers.Length; i++)
                    {
                        target.Cells[1, i + 1].Value = headers[i];
                    }

                    //変換テーブルの作成
                    int[,] exchange ={
                        {1,13 },
                        {5,33 },
                        {11,53 },
                        {16,73 }
                    };

                    //値の当て込み
                    for (int i = 0; i < exchange.GetLength(0); i++)
                    {
                        target.Cells[2, exchange[i, 0], endRow, exchange[i, 0]].Value =
                        csv.Cells[1, exchange[i, 1], endRow, exchange[i, 1]].Value;
                    }

                    //TPiCS用ステータス書き込み
                    
                    target.Cells[2, 9, endRow, 9].Value = "J";
                    

                    //CSVシート削除
                    package.Workbook.Worksheets.Delete(csv);

                    //XLSX保存
                    package.Save();
                    File.Delete(ExportSourceFolder + ExportSourceFileName);
                    ReloadFilesExist();

                    //CSVファイルに書き込むときに使うEncoding
                    Encoding enc = Encoding.GetEncoding("Shift_JIS");

                    //CSV書き込み
                    using (StreamWriter sr = new StreamWriter(ExportCsvFolder + ExportCsvFileName, false, enc))
                    {
                        var endCol = target.Dimension.End.Column;
                        endRow = target.Dimension.End.Row;

                        for (int i = 1; i <= endRow; i++)
                        {
                            for (int ii = 1; ii < endCol; ii++)
                            {
                                sr.Write(target.Cells[i, ii].Value);
                                if (ii != endCol - 1) sr.Write(",");
                            }
                            sr.Write("\r\n");
                        }
                    }
                }
            });
            IsDialogOpen = false;
            ShowSnackbar("正常終了しました。");
        }
        /// <summary>
        /// ファイル上賀きモードでインポートデータ作成を実行
        /// </summary>
        private async void ImportWhithinOverWrite()
        {
            DialogContent = new LoadingProgressDialog();
            IsDialogOpen = true;
            await Task.Run(() =>
            {
                var csvFile = new FileInfo(ImportCsvFolder + ImportCsvFileName);
                if (csvFile.Exists)
                {
                    csvFile.Delete();
                    csvFile = new FileInfo(ImportCsvFolder + ImportCsvFileName);
                }

                var xlsxFile = new FileInfo(Environment.CurrentDirectory + "temp.xlsx");
                if (xlsxFile.Exists)
                {
                    xlsxFile.Delete();
                    xlsxFile = new FileInfo(Environment.CurrentDirectory + "temp.xlsx");
                }

                //Excelパッケージ作成
                using (ExcelPackage package = new ExcelPackage(xlsxFile))
                {
                    //SQL用シート作成
                    ExcelWorksheet SQL = package.Workbook.Worksheets.Add("sql");

                    var server = Settings.Default.TpicsDbIP;
                    var name = Settings.Default.TpicsDbName;
                    var user = Settings.Default.TpicsDbUser;
                    var pass = Settings.Default.TpicsDbPass;

                    var cs = "Data Source = " + server
                        + ";Initial Catalog = " + name
                        + ";User ID = " + user
                        + ";Password = " + pass;

                    using (SqlConnection con = new SqlConnection(cs))
                    {
                        con.Open();
                        SqlCommand cmd = con.CreateCommand();
                        cmd.CommandText = Settings.Default.InspectionImportSql;
                        SqlDataReader reader = cmd.ExecuteReader();
                        //シートにリーダーからロード
                        SQL.Cells["A1"].LoadFromDataReader(reader, true);
                    }


                    //変換用シート作成
                    ExcelWorksheet target = package.Workbook.Worksheets.Add("sheet1");

                    //最終行取得
                    int ER = SQL.Dimension.End.Row;

                    //ヘッダテーブル
                    string[] Title =
                    {
                        "01_新規・変更・削除区分","02_仕入先コード","03_仕入先名","04_仕入先情報（コード）２","05_仕入先情報（名称）２",
                        "06_仕入先情報（予備）１","07_仕入先情報（予備）２","08_仕入先情報（予備）３","09_仕入先情報（予備）４","10_入荷予定日",
                        "11_日付２","12_日付３","13_発注番号","14_発注番号行番号","15_予定列番号１",
                        "16_予定番号２","17_予定行番号２","18_予定列番号２","19_予定番号３","20_予定行番号３",
                        "21_予定列番号３","22_バッチ番号１","23_バッチ番号２","24_コード１","25_名称１",
                        "26_コード２","27_名称２","28_コード３","29_名称３","30_ロケーション",
                        "31_ロケーション予備１","32_ロケーション予備２","33_品番","34_品番予備１","35_品番予備２",
                        "36_品名","37_品名予備１","38_品名予備２","39_規格","40_規格予備１",
                        "41_規格予備２","42_ＨＴ画面表示ソートキー","43_文字備考２","44_文字備考３","45_数値備考１",
                        "46_数値備考２","47_数値備考３","48_バーコード１","49_バーコード２","50_バーコード３",
                        "51_入数１","52_入数２","53_総バラ入荷予定数","54_ケース入荷予定数","55_ボール入荷予定数",
                        "56_バラ出荷予定数","57_予定数５","58_ロット管理区分１","59_ロット管理区分２（予備）","60_ロット管理区分３（予備）",
                        "61_ロットチェック値１","62_ロットチェック値２（予備）","63_ロットチェック値３（予備）","64_ロットチェックフラグ１","65_ロットチェックフラグ２",
                        "66_ロットチェックフラグ３（予備）","67_倉庫コード"
                    };

                    //ヘッダ当て込み
                    for (int i = 0; i < Title.Length; i++)
                    {
                        target.Cells[1, i + 1].Value = Title[i];
                    }

                    //変換テーブル{csv列,変換列}
                    int[,] Exchange =
                    {
                        { 2, 8},
                        { 3,64},
                        {10,15},
                        {13,1 },
                        {30,37},
                        {33,4 },
                        {36,61},
                        {48,4 },
                        {53,9 }
                    };

                    //データ挿入
                    for (int i = 0; i < Exchange.GetLength(0); i++)
                    {
                        target.Cells[2, Exchange[i, 0], ER, Exchange[i, 0]].Value =
                           SQL.Cells[2, Exchange[i, 1], ER, Exchange[i, 1]].Value;
                    }

                    //日付　最後の１を消す
                    string s;
                    for (int ii = 2; ii <= ER; ii++)
                    {
                        s = target.Cells[ii, 10].Text;
                        target.Cells[ii, 10].Value = s.Substring(0, 8);
                    }

                    //バイト数で切り捨て
                    for (int ii = 2; ii <= ER; ii++)
                    {
                        string text = target.Cells[ii, 36].Text;
                        int size = 40; // 40バイトで切り捨て
                        Encoding e = System.Text.Encoding.GetEncoding("Shift_JIS");
                        string result = new String(text.TakeWhile((c, i) => e.GetByteCount(text.Substring(0, i + 1)) <= size).ToArray());

                        target.Cells[ii, 36].Value = result;
                    }

                    //規定値入力
                    target.Cells[2, 14, ER, 14].Value = 1;

                    //SQLシート削除
                    package.Workbook.Worksheets.Delete(SQL);

                    //保存処理
                    package.Save();

                    ReloadFilesExist();

                    //CSVファイルに書き込むときに使うEncoding
                    Encoding enc = Encoding.GetEncoding("Shift_JIS");

                    //CSV書き込み
                    using (StreamWriter sr = new StreamWriter(ImportCsvFolder + ImportCsvFileName, false, enc))
                    {
                        var endCol = target.Dimension.End.Column;
                        var endRow = target.Dimension.End.Row;

                        for (int i = 1; i < endRow + 1; i++)
                        {
                            for (int ii = 1; ii < endCol + 1; ii++)
                            {
                                sr.Write(target.Cells[i, ii].Value);
                                if (ii != endCol) sr.Write(",");
                            }
                            if (i != endRow) sr.Write("\r\n");
                        }
                    }
                }
            });
            IsDialogOpen = false;
            ShowSnackbar("正常終了しました。");
        }



        private ICommand _reloadFilesExistCommand;
        public ICommand ReloadFilesExistCommand
        {
            get
            {
                if (_reloadFilesExistCommand != null) return _reloadFilesExistCommand;
                _reloadFilesExistCommand = new RelayCommand<object>(ExecuteReloadFilesExistCommand);
                return _reloadFilesExistCommand;
            }
        }
        public void ExecuteReloadFilesExistCommand(object arg)
        {
            ReloadFilesExist();
        }

        private ICommand _openExportFilesCommand;
        public ICommand OpenExportFilesCommand
        {
            get
            {
                if (_openExportFilesCommand != null) return _openExportFilesCommand;
                _openExportFilesCommand = new RelayCommand<string>(ExecuteOpenExportFilesCommand);
                return _openExportFilesCommand;
            }
        }
        public void ExecuteOpenExportFilesCommand(string arg)
        {
            switch (arg)
            {
                case "Source":
                    FileOpen(ExportSourceFolder + ExportSourceFileName);
                    break;
                case "XLSX":
                    FileOpen(ExportXlsxFolder + ExportXlsxFileName);
                    break;
                case "CSV":
                    FileOpen(ExportCsvFolder + ExportCsvFileName);
                    break;
                case "ImportCSV":
                    FileOpen(ImportCsvFolder + ImportCsvFileName);
                    break;
            }
        }

        private ICommand _exportWithinOrverWriteCommand;
        public ICommand ExportWithinOrverWriteCommand
        {
            get
            {
                if (_exportWithinOrverWriteCommand != null) return _exportWithinOrverWriteCommand;
                _exportWithinOrverWriteCommand = new RelayCommand<object>(ExecuteExportWithinOrverWriteCommand, CanExportOutputWithinOrverWriteCommand);
                return _exportWithinOrverWriteCommand;
            }
        }
        public void ExecuteExportWithinOrverWriteCommand(object arg)
        {
            ExportWhithinOverWrite();
        }
        private bool CanExportOutputWithinOrverWriteCommand(object arg)
        {
            ReloadFilesExist();
            return ExportSourceFileExist != notExist;
        }

        private ICommand _importWithinOrverWriteCommand;
        public ICommand ImportWithinOrverWriteCommand
        {
            get
            {
                if (_importWithinOrverWriteCommand != null) return _importWithinOrverWriteCommand;
                _importWithinOrverWriteCommand = new RelayCommand<object>(ExecuteImportWithinOrverWriteCommand, CanExportImportWithinOrverWriteCommand);
                return _importWithinOrverWriteCommand;
            }
        }
        public void ExecuteImportWithinOrverWriteCommand(object arg)
        {
            ImportWhithinOverWrite();
        }
        private bool CanExportImportWithinOrverWriteCommand(object arg)
        {
            ReloadFilesExist();
            return HasErrors == false;
        }

    }
}
