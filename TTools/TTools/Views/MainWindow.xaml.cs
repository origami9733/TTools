using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Threading;
using MaterialDesignThemes.Wpf;
using TTools.ViewModels;

namespace TTools.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //ViewModelインスタンスをデータコンテクストに投入
            this.DataContext = new MainWindowVM();

            //トースト通知を非同期表示
            Task.Factory.StartNew(() => { Thread.Sleep(2500); })
                .ContinueWith(t => { MainSnackbar.MessageQueue.Enqueue("更新情報はありません。"); }
                , TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;
        }

        //画面右上のポップアップメニューのボタン押下イベント
        private async void MenuPopupButton_OnClick_Async(object sender, RoutedEventArgs e)
        {
            var sampleMessageDialog = new LoadingMessageDialog
            {
                Message = { Text = ((ButtonBase)sender).Content.ToString() }
            };

            await DialogHost.Show(sampleMessageDialog, "RootDialog");
        }
    }
}
