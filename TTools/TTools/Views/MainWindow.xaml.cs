using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using TTools.ViewModels;


namespace TTools.Views
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            DataContext = new MainWindowVM();
            InitializeComponent();
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //var dependencyObject = Mouse.Captured as DependencyObject;
            //while (dependencyObject != null)
            //{
            //    if (dependencyObject is ScrollBar) return;
            //    dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            //}

            MenuToggleButton.IsChecked = false;
        }
    }
}
