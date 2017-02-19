using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTools.Models;
using TTools.Properties;

namespace TTools.Domain
{
    public class BeautyGhContext
    {
        //Beauty社のDBから受注情報をロードする
        public DispatchObservableCollection<OrderItem> Load(string sqlStr)
        {
            //Setting読み込み
            string dbServer = Settings.Default.BeautyDbIP;
            string dbName = Settings.Default.BeautyDbName;
            string dbUser = Settings.Default.BeautyDbUser;
            string dbPassword = Settings.Default.BeautyDbPass;


            //接続文字列
            string conString =
                "Data Source = " + dbServer + ";" +
                "Initial Catalog = " + dbName + ";" +
                "User ID = " + dbUser + ";" +
                "Password = " + dbPassword;


            //SQL文字列
            string sqlString = sqlStr;


            //DataTableへ読み込み
            DataTable dataTable;
            using (SqlConnection con = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand(sqlString, con);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);

                dataTable = new DataTable();

                sqlDataAdapter.Fill(dataTable);
            }


            //ObservableCollectionにマッピングする
            DispatchObservableCollection<OrderItem> items = new DispatchObservableCollection<OrderItem>();
            foreach (DataRow row in dataTable.Rows)
            {
                items.Add(new OrderItem
                {
                    伝票ＮＯ = (string)row["伝票ＮＯ"],
                    営業所コード = (string)row["営業所コード"],
                    営業所名 = (string)row["営業所名"],
                    営業所電話番号 = (string)row["営業所電話番号"],
                    物流発注年月日 = (string)row["物流発注年月日"],
                    発注先コード = (string)row["発注先コード"],
                    発注先名称 = (string)row["発注先名称"],
                    仕入先コード = (string)row["仕入先コード"],
                    仕入先名称 = (string)row["仕入先名称"],
                    納入先コード = (string)row["納入先コード"],
                    納入先名称 = (string)row["納入先名称"],
                    納入先ＳＳ名称 = (string)row["納入先ＳＳ名称"],
                    納入先マークコード = (string)row["納入先マークコード"],
                    納入先マーク名 = (string)row["納入先マーク名"],
                    納入先郵便番号 = (string)row["納入先郵便番号"],
                    納入先住所_市町村名 = (string)row["納入先住所_市町村名"],
                    納入先住所_番地 = (string)row["納入先住所_番地"],
                    納入先住所_ビル名 = (string)row["納入先住所_ビル名"],
                    納入先電話番号 = (string)row["納入先電話番号"],
                    機種コード = (string)row["機種コード"],
                    機種名 = (string)row["機種名"],
                    機番 = (string)row["機番"],
                    商品コード = (string)row["商品コード"],
                    商品名 = (string)row["商品名"],
                    仕様_備考 = (string)row["仕様_備考"],
                    業者図番 = (string)row["業者図番"],
                    発注数量 = (string)row["発注数量"],
                    発注単価 = (string)row["発注単価"],
                    発注金額 = (string)row["発注金額"],
                    サイクル区分 = (string)row["サイクル区分"],
                    送区分 = (string)row["送区分"],
                    発注入力振分区分 = (string)row["発注入力振分区分"],
                    発注入力振分グループ番号 = (string)row["発注入力振分グループ番号"],
                    周辺機器グループ番号 = (string)row["周辺機器グループ番号"],
                    発注回数 = (string)row["発注回数"],
                    元_伝票ＮＯ = (string)row["元_伝票ＮＯ"],
                    要請年月日 = (string)row["要請年月日"],
                    契約番号 = (string)row["契約番号"],
                    フラグ１ = (string)row["フラグ１"],
                    フラグ２ = (string)row["フラグ２"],
                    フラグ３ = (string)row["フラグ３"],
                    フラグ４ = (string)row["フラグ４"],
                    フラグ５ = (string)row["フラグ５"],
                    フラグ６ = (string)row["フラグ６"],
                    フラグ７ = (string)row["フラグ７"],
                    フラグ８ = (string)row["フラグ８"],
                    フラグ９ = (string)row["フラグ９"],
                    フラグ１０ = (string)row["フラグ１０"],
                    業者連絡事項 = (string)row["業者連絡事項"],
                    伝送フラグ = (short)row["伝送フラグ"],
                    更新フラグ = (string)row["更新フラグ"],
                    更新者 = (short)row["更新者"],
                    更新日付 = (string)row["更新日付"],
                    更新時刻 = (string)row["更新時刻"],
                    発注連絡事項 = (string)row["発注連絡事項"],
                });
            }

            return items;
        }
    }
}
