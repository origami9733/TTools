using System.Data;
using System.Data.SqlClient;
using TTools.Models;
using TTools.Properties;

namespace TTools.Domain
{
    public static class TpicsDbContext
    {
        /// <summary>
        /// ベンダーテーブルのロード
        /// </summary>
        /// <returns></returns>
        public static DispatchObservableCollection<VendorItem> LoadVendor()
        {
            //Setting読み込み
            string dbServer = Settings.Default.TpicsDbIP;
            string dbName = Settings.Default.TpicsDbName;
            string dbUser = Settings.Default.TpicsDbUser;
            string dbPassword = Settings.Default.TpicsDbPass;

            //接続文字列
            string conString =
                "Data Source = " + dbServer + ";" +
                "Initial Catalog = " + dbName + ";" +
                "User ID = " + dbUser + ";" +
                "Password = " + dbPassword;

            //SQL文字列
            string sqlString = "SELECT * FROM dbo.XSECT";

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
            DispatchObservableCollection<VendorItem> items = new DispatchObservableCollection<VendorItem>();
            foreach (DataRow row in dataTable.Rows)
            {
                items.Add(new VendorItem
                {
                    Id = row["BUMO"] as string,
                    Name = row["BNAME"] as string,
                    Tel = row["TEL"] as string,
                    Fax = row["FAX"] as string,
                    Zip = row["ZIP"] as string,
                    Adr = row["ADR1"] as string,
                    Mail = row["MAIL"] as string,
                });
            }

            return items;
        }

        /// <summary>
        /// カレンダーの取得
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DataTable LoadCalendar(string dateTime)
        {
            //Setting読み込み
            string dbServer = Settings.Default.TpicsDbIP;
            string dbName = Settings.Default.TpicsDbName;
            string dbUser = Settings.Default.TpicsDbUser;
            string dbPassword = Settings.Default.TpicsDbPass;

            //接続文字列
            string conString =
                "Data Source = " + dbServer + ";" +
                "Initial Catalog = " + dbName + ";" +
                "User ID = " + dbUser + ";" +
                "Password = " + dbPassword;

            //SQL文字列
            string sqlString = "SELECT * FROM dbo.XCALE WHERE CALENAME >= '" + dateTime + "'";

            //DataTableへ読み込み
            DataTable dataTable;
            using (SqlConnection con = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand(sqlString, con);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);

                dataTable = new DataTable();

                sqlDataAdapter.Fill(dataTable);
            }

            return dataTable;
        }
    }
}
