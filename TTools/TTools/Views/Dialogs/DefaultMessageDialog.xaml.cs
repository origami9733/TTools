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

namespace TTools.Views
{
    /// <summary>
    /// MessageDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class DefaultMessageDialog : UserControl
    {
        public DefaultMessageDialog()
        {
            InitializeComponent();
        }

        public bool Accept { get; set; }

        private void AcceptBT_Click(object sender, RoutedEventArgs e)
        {
            Accept = true;
        }

        private void CancelBT_Click(object sender, RoutedEventArgs e)
        {
            Accept = false;
        }
    }
}
