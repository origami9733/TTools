using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TTools.Models;

namespace TTools.Domain
{
    public class MachineOrderItemsToCount : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            String msg;
            var args = (ReadOnlyObservableCollection<object>)value;
            var items = new ObservableCollection<DisplayMachineOrderManagementItem>();

            if (args.Count() == 0) return null;

            foreach (var a in args)
            {
                items.Add((DisplayMachineOrderManagementItem)a);
            }

            if (items.Count() == 0) return null;

            var p = (string)parameter;

            switch (p)
            {
                case "Count":
                    msg = ReturnCount(items);
                    break;
                case "OrderId":
                    msg = ReturnOrderId(items);
                    break;
                case "ShortName":
                    var item = ReturnShortName(items);
                    msg = item.ShortName;
                    break;
                case "SumPrice":
                    msg = ReturnSumPrice(items);
                    break;
                default:
                    msg = null;
                    break;
            }

            return msg;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            String msg;
            var args = (ReadOnlyObservableCollection<object>)value;
            var items = new ObservableCollection<DisplayMachineOrderManagementItem>();

            if (args.Count() == 0) return null;

            foreach (var a in args)
            {
                items.Add((DisplayMachineOrderManagementItem)a);
            }

            if (items.Count() == 0) return null;

            var p = (string)parameter;

            switch (p)
            {
                case "Count":
                    msg = ReturnCount(items);
                    break;
                case "OrderId":
                    msg = ReturnOrderId(items);
                    break;
                case "ShortName":
                    var item = ReturnShortName(items);
                    msg = item.ShortName;
                    break;
                case "SumPrice":
                    msg = ReturnSumPrice(items);
                    break;
                default:
                    msg = null;
                    break;
            }

            return msg;
        }

        /// <summary>
        /// 要素数を返す
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private string ReturnCount(ObservableCollection<DisplayMachineOrderManagementItem> items)
        {
            return items.Count().ToString();
        }


        /// <summary>
        /// オーダーIDを返す
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private string ReturnOrderId(ObservableCollection<DisplayMachineOrderManagementItem> items)
        {
            var item = items[0];
            return item.OrderItem.伝票ＮＯ.Substring(0, 10);
        }

        /// <summary>
        /// 短縮名を返す
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private ProductItem ReturnShortName(ObservableCollection<DisplayMachineOrderManagementItem> items)
        {
            var item = items.Where(x => x.OrderItem.伝票ＮＯ.Substring(11, 2) == "01").First();
            return item.ProductItem;
        }

        /// <summary>
        /// 合計単価を返す
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private string ReturnSumPrice(ObservableCollection<DisplayMachineOrderManagementItem> items)
        {
            return items.Sum(x => x.ProductItem.Price).ToString();
        }
    }
}
