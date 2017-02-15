using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TTools.ViewModels;

namespace TTools.Views
{
    /// <summary>
    /// Jyuchu.xaml の相互作用ロジック
    /// </summary>
    public partial class ImportOrders: UserControl
    {
        ImportOrdersVM vm = new ImportOrdersVM();

        public ImportOrders()
        {
            InitializeComponent();
            DataContext = vm;
            CheckBox1.DataContext = vm;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            this.DG1.CancelEdit();
        }
    }
}
