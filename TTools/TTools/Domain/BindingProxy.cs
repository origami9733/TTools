using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TTools.Domain
{
    /// <summary>
    /// VMのバインディングソースの代理として働くクラス。
    /// </summary>
    public class BindingProxy : Freezable
    {
        /// <summary>
        /// Freezableオブジェクトのインスタンスを作成します。
        /// </summary>
        /// <returns></returns>
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        /// <summary>
        /// 間をとりもつプロパティ
        /// データバインドした場合は、このプロパティがVMの代わりになる。
        /// </summary>
        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        /// <summary>
        /// Dataの依存関係プロパティ定義
        /// </summary>
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));
    }
}
