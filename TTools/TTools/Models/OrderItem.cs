using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TTools.Domain;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using TTools.Properties;

namespace TTools.Models
{
    public class OrderItem : INotifyPropertyChanged
    {
        #region 変数
        //エンティティ
        private string _伝票ＮＯ;
        private string _営業所コード;
        private string _営業所名;
        private string _営業所電話番号;
        private string _物流発注年月日;
        private string _発注先コード;
        private string _発注先名称;
        private string _仕入先コード;
        private string _仕入先名称;
        private string _納入先コード;
        private string _納入先名称;
        private string _納入先ＳＳ名称;
        private string _納入先マークコード;
        private string _納入先マーク名;
        private string _納入先郵便番号;
        private string _納入先住所_市町村名;
        private string _納入先住所_番地;
        private string _納入先住所_ビル名;
        private string _納入先電話番号;
        private string _機種コード;
        private string _機種名;
        private string _機番;
        private string _商品コード;
        private string _商品名;
        private string _仕様_備考;
        private string _業者図番;
        private string _発注数量;
        private string _発注単価;
        private string _発注金額;
        private string _サイクル区分;
        private string _送区分;
        private string _発注入力振分区分;
        private string _発注入力振分グループ番号;
        private string _周辺機器グループ番号;
        private string _発注回数;
        private string _元_伝票ＮＯ;
        private string _要請年月日;
        private string _契約番号;
        private string _フラグ１;
        private string _フラグ２;
        private string _フラグ３;
        private string _フラグ４;
        private string _フラグ５;
        private string _フラグ６;
        private string _フラグ７;
        private string _フラグ８;
        private string _フラグ９;
        private string _フラグ１０;
        private string _業者連絡事項;
        private short _伝送フラグ;
        private string _更新フラグ;
        private short _更新者;
        private string _更新日付;
        private string _更新時刻;
        private string _発注連絡事項;
        #endregion
        #region プロパティ
        [Key]
        public string 伝票ＮＯ
        {
            get { return _伝票ＮＯ; }
            set
            {
                if (_伝票ＮＯ != value)
                {
                    _伝票ＮＯ = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 営業所コード
        {
            get { return _営業所コード; }
            set
            {
                if (_営業所コード != value)
                {
                    _営業所コード = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 営業所名
        {
            get { return _営業所名; }
            set
            {
                if (_営業所名 != value)
                {
                    _営業所名 = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 営業所電話番号
        {
            get { return _営業所電話番号; }
            set
            {
                if (_営業所電話番号 != value)
                {
                    _営業所電話番号 = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 物流発注年月日
        {
            get { return _物流発注年月日; }
            set
            {
                if (_物流発注年月日 != value)
                {
                    _物流発注年月日 = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 発注先コード
        {
            get { return _発注先コード; }
            set
            {
                if (_発注先コード != value)
                {
                    _発注先コード = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 発注先名称
        {
            get { return _発注先名称; }
            set
            {
                if (_発注先名称 != value)
                {
                    _発注先名称 = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 仕入先コード
        {
            get { return _仕入先コード; }
            set
            {
                if (_仕入先コード != value)
                {
                    _仕入先コード = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 仕入先名称
        {
            get { return _仕入先名称; }
            set
            {
                if (_仕入先名称 != value)
                {
                    _仕入先名称 = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 納入先コード
        {
            get { return _納入先コード; }
            set
            {
                if (_納入先コード != value)
                {
                    _納入先コード = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 納入先名称
        {
            get { return _納入先名称; }
            set
            {
                if (_納入先名称 != value)
                {
                    _納入先名称 = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 納入先ＳＳ名称
        {
            get { return _納入先ＳＳ名称; }
            set
            {
                if (_納入先ＳＳ名称 != value)
                {
                    _納入先ＳＳ名称 = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 納入先マークコード
        {
            get { return _納入先マークコード; }
            set
            {
                if (_納入先マークコード != value)
                {
                    _納入先マークコード = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 納入先マーク名
        {
            get { return _納入先マーク名; }
            set
            {
                if (_納入先マーク名 != value)
                {
                    _納入先マーク名 = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 納入先郵便番号
        {
            get { return _納入先郵便番号; }
            set
            {
                if (_納入先郵便番号 != value)
                {
                    _納入先郵便番号 = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 納入先住所_市町村名
        {
            get { return _納入先住所_市町村名; }
            set
            {
                if (_納入先住所_市町村名 != value)
                {
                    _納入先住所_市町村名 = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 納入先住所_番地
        {
            get { return _納入先住所_番地; }
            set
            {
                if (_納入先住所_番地 != value)
                {
                    _納入先住所_番地 = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 納入先住所_ビル名
        {
            get { return _納入先住所_ビル名; }
            set
            {
                if (_納入先住所_ビル名 != value)
                {
                    _納入先住所_ビル名 = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 納入先電話番号
        {
            get { return _納入先電話番号; }
            set
            {
                if (_納入先電話番号 != value)
                {
                    _納入先電話番号 = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 機種コード
        {
            get { return _機種コード; }
            set
            {
                if (_機種コード != value)
                {
                    _機種コード = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 機種名
        {
            get { return _機種名; }
            set
            {
                if (_機種名 != value)
                {
                    _機種名 = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 機番
        {
            get { return _機番; }
            set
            {
                if (_機番 != value)
                {
                    _機番 = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 商品コード
        {
            get { return _商品コード; }
            set
            {
                if (_商品コード != value)
                {
                    _商品コード = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 商品名
        {
            get { return _商品名; }
            set
            {
                if (_商品名 != value)
                {
                    _商品名 = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 仕様_備考
        {
            get { return _仕様_備考; }
            set
            {
                if (_仕様_備考 != value)
                {
                    _仕様_備考 = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string 業者図番
        {
            get { return _業者図番; }
            set
            {
                if (_業者図番 != value)
                {
                    _業者図番 = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string 発注数量
        {
            get { return _発注数量; }
            set
            {
                if (_発注数量 != value)
                {
                    _発注数量 = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string 発注単価
        {
            get { return _発注単価; }
            set
            {
                if (_発注単価 != value)
                {
                    _発注単価 = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string 発注金額
        {
            get { return _発注金額; }
            set
            {
                if (_発注金額 != value)
                {
                    _発注金額 = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string サイクル区分
        {
            get { return _サイクル区分; }
            set
            {
                if (_サイクル区分 != value)
                {
                    _サイクル区分 = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string 送区分
        {
            get { return _送区分; }
            set
            {
                if (_送区分 != value)
                {
                    _送区分 = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string 発注入力振分区分
        {
            get { return _発注入力振分区分; }
            set
            {
                if (_発注入力振分区分 != value)
                {
                    _発注入力振分区分 = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string 発注入力振分グループ番号
        {
            get { return _発注入力振分グループ番号; }
            set
            {
                if (_発注入力振分グループ番号 != value)
                {
                    _発注入力振分グループ番号 = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string 周辺機器グループ番号
        {
            get { return _周辺機器グループ番号; }
            set
            {
                if (_周辺機器グループ番号 != value)
                {
                    _周辺機器グループ番号 = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string 発注回数
        {
            get { return _発注回数; }
            set
            {
                if (_発注回数 != value)
                {
                    _発注回数 = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string 元_伝票ＮＯ
        {
            get { return _元_伝票ＮＯ; }
            set
            {
                if (_元_伝票ＮＯ != value)
                {
                    _元_伝票ＮＯ = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string 要請年月日
        {
            get { return _要請年月日; }
            set
            {
                if (_要請年月日 != value)
                {
                    _要請年月日 = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string 契約番号
        {
            get { return _契約番号; }
            set
            {
                if (_契約番号 != value)
                {
                    _契約番号 = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string フラグ１
        {
            get { return _フラグ１; }
            set
            {
                if (_フラグ１ != value)
                {
                    _フラグ１ = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string フラグ２
        {
            get { return _フラグ２; }
            set
            {
                if (_フラグ２ != value)
                {
                    _フラグ２ = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string フラグ３
        {
            get { return _フラグ３; }
            set
            {
                if (_フラグ３ != value)
                {
                    _フラグ３ = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string フラグ４
        {
            get { return _フラグ４; }
            set
            {
                if (_フラグ４ != value)
                {
                    _フラグ４ = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string フラグ５
        {
            get { return _フラグ５; }
            set
            {
                if (_フラグ５ != value)
                {
                    _フラグ５ = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string フラグ６
        {
            get { return _フラグ６; }
            set
            {
                if (_フラグ６ != value)
                {
                    _フラグ６ = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string フラグ７
        {
            get { return _フラグ７; }
            set
            {
                if (_フラグ７ != value)
                {
                    _フラグ７ = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string フラグ８
        {
            get { return _フラグ８; }
            set
            {
                if (_フラグ８ != value)
                {
                    _フラグ８ = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string フラグ９
        {
            get { return _フラグ９; }
            set
            {
                if (_フラグ９ != value)
                {
                    _フラグ９ = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string フラグ１０
        {
            get { return _フラグ１０; }
            set
            {
                if (_フラグ１０ != value)
                {
                    _フラグ１０ = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string 業者連絡事項
        {
            get { return _業者連絡事項; }
            set
            {
                if (_業者連絡事項 != value)
                {
                    _業者連絡事項 = value;
                    RaisePropertyChanged();
                }
            }
        }

        public short 伝送フラグ
        {
            get { return _伝送フラグ; }
            set
            {
                if (_伝送フラグ != value)
                {
                    _伝送フラグ = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string 更新フラグ
        {
            get { return _更新フラグ; }
            set
            {
                if (_更新フラグ != value)
                {
                    _更新フラグ = value;
                    RaisePropertyChanged();
                }
            }
        }

        public short 更新者
        {
            get { return _更新者; }
            set
            {
                if (_更新者 != value)
                {
                    _更新者 = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string 更新日付
        {
            get { return _更新日付; }
            set
            {
                if (_更新日付 != value)
                {
                    _更新日付 = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string 更新時刻
        {
            get { return _更新時刻; }
            set
            {
                if (_更新時刻 != value)
                {
                    _更新時刻 = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string 発注連絡事項
        {
            get { return _発注連絡事項; }
            set
            {
                if (_発注連絡事項 != value)
                {
                    _発注連絡事項 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        private string _orderStatus;
        private string _shippingCompanyName;
        private string _shippingCompanyTel;
        private string _invoiceNo;

        public string OrderStatus
        {
            get { return _orderStatus; }
            set
            {
                if (_orderStatus == value) return;
                _orderStatus = value;
                RaisePropertyChanged();
            }
        }
        public string ShippingCompanyName
        {
            get { return _shippingCompanyName; }
            set
            {
                if (_shippingCompanyName == value) return;
                _shippingCompanyName = value;
                RaisePropertyChanged();
            }
        }
        public string ShippingCompanyTel
        {
            get { return _shippingCompanyTel; }
            set
            {
                if (_shippingCompanyTel == value) return;
                _shippingCompanyTel = value;
                RaisePropertyChanged();
            }
        }
        public string InvoiceNo
        {
            get { return _invoiceNo; }
            set
            {
                if (_invoiceNo == value) return;
                _invoiceNo = value;
                RaisePropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}