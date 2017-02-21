using System.Windows;
using System.Windows.Controls;

namespace TTools.Views
{
    public partial class ImportOrder: UserControl
    {
        public ImportOrder()
        {
            InitializeComponent();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            DG1.CancelEdit();
        }
    }
}
